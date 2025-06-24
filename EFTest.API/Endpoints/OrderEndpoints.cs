using FluentValidation;
using MediatR;
using EFTest.Application.Commands;
using EFTest.Application.Queries;
using EFTest.Application.DTOs;

namespace EFTest.API.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/api/orders")
            .WithTags("Orders")
            .WithOpenApi();

        // GET /api/orders
        orders.MapGet("/", GetAllOrders)
            .WithName("GetAllOrders")
            .WithSummary("Get all orders")
            .WithDescription(@"
Retrieves all orders from the system with their embedded order lines.

**DDD Architecture Flow:**
1. API receives request
2. Creates GetAllOrdersQuery
3. MediatR sends to GetAllOrdersHandler
4. Handler calls OrderRepository
5. Repository maps persistence entities to domain entities
6. Handler maps domain entities to DTOs
7. Returns structured response

**Response includes:**
- Order details (ID, date, customer name, total amount)
- Embedded order lines with calculated line totals
- Currency information for international support")
            .Produces<List<OrderDto>>(200)
            .Produces(500);

        // GET /api/orders/{id}
        orders.MapGet("/{id:guid}", GetOrderById)
            .WithName("GetOrderById")
            .WithSummary("Get order by ID")
            .WithDescription(@"
Retrieves a specific order by its unique GUID identifier.

**Parameters:**
- `id`: The unique GUID identifier of the order

**DDD Architecture:**
- Uses domain repository pattern
- Maps through application layer DTOs
- Maintains clean separation of concerns")
            .Produces<OrderDto>(200)
            .Produces(404)
            .Produces(500);

        // POST /api/orders
        orders.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithSummary("Create a new order")
            .WithDescription(@"
Creates a new order with embedded order lines using Domain Driven Design patterns.

**DDD Command Flow:**
1. Request mapped to CreateOrderCommand
2. FluentValidation validates the command
3. MediatR sends to CreateOrderHandler
4. Handler creates rich domain entities (Order, OrderLine)
5. Domain validates business invariants
6. OrderDomainService applies business rules
7. Repository persists through infrastructure layer
8. Response mapped to DTO

**Business Rules Enforced:**
- Customer name must be valid (letters, spaces, hyphens, periods only)
- Order must contain at least one order line
- Maximum 50 order lines per order
- Quantity must be positive and reasonable
- Unit prices must be positive
- Total amount automatically calculated

**Value Objects Used:**
- CustomerName: Validates customer name format
- ProductName: Validates product name format  
- Quantity: Ensures positive quantities within limits
- Money: Handles currency and decimal precision")
            .Accepts<CreateOrderDto>("application/json")
            .Produces<OrderDto>(201)
            .Produces(400)
            .Produces(500);

        // PUT /api/orders/{id}
        orders.MapPut("/{id:guid}", UpdateOrder)
            .WithName("UpdateOrder")
            .WithSummary("Update an existing order")
            .WithDescription(@"
Updates an existing order and its embedded order lines.

**DDD Update Flow:**
1. Validates order exists
2. Creates UpdateOrderCommand
3. Domain entities enforce business rules
4. Repository updates through infrastructure
5. Maintains data consistency

**Note:** This replaces all order lines with the provided ones.")
            .Accepts<UpdateOrderDto>("application/json")
            .Produces(204)
            .Produces(400)
            .Produces(404)
            .Produces(500);

        // DELETE /api/orders/{id}
        orders.MapDelete("/{id:guid}", DeleteOrder)
            .WithName("DeleteOrder")
            .WithSummary("Delete an order")
            .WithDescription(@"
Deletes an order and all its embedded order lines.

**DDD Delete Flow:**
1. Creates DeleteOrderCommand
2. MediatR sends to DeleteOrderHandler
3. Repository removes order and embedded lines atomically
4. Returns success status

**Note:** This operation is permanent and cannot be undone.")
            .Produces(204)
            .Produces(404)
            .Produces(500);
    }

    private static async Task<IResult> GetAllOrders(
        IMediator mediator,
        ILogger<Program> logger)
    {
        try
        {
            var query = new GetAllOrdersQuery();
            var orders = await mediator.Send(query);
            return Results.Ok(orders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving orders");
            return Results.Problem("Internal server error", statusCode: 500);
        }
    }

    private static async Task<IResult> GetOrderById(
        Guid id,
        IMediator mediator,
        ILogger<Program> logger)
    {
        try
        {
            var query = new GetOrderByIdQuery { Id = id };
            var order = await mediator.Send(query);

            if (order == null)
            {
                return Results.NotFound($"Order with ID {id} not found");
            }

            return Results.Ok(order);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving order {OrderId}", id);
            return Results.Problem("Internal server error", statusCode: 500);
        }
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderDto createOrderDto,
        IMediator mediator,
        IValidator<CreateOrderCommand> validator,
        ILogger<Program> logger)
    {
        try
        {
            // Map DTO to command
            var command = new CreateOrderCommand
            {
                CustomerName = createOrderDto.CustomerName,
                OrderDate = createOrderDto.OrderDate,
                OrderLines = createOrderDto.OrderLines
            };

            // Validate the command
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return Results.ValidationProblem(errors);
            }

            // Send command through MediatR
            var order = await mediator.Send(command);

            return Results.CreatedAtRoute("GetOrderById", new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid argument when creating order");
            return Results.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Business rule violation when creating order");
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order");
            return Results.Problem("Internal server error", statusCode: 500);
        }
    }

    private static async Task<IResult> UpdateOrder(
        Guid id,
        UpdateOrderDto updateOrderDto,
        IMediator mediator,
        ILogger<Program> logger)
    {
        try
        {
            if (id != updateOrderDto.Id)
            {
                return Results.BadRequest("Order ID mismatch");
            }

            // Map DTO to command
            var command = new UpdateOrderCommand
            {
                Id = updateOrderDto.Id,
                CustomerName = updateOrderDto.CustomerName,
                OrderDate = updateOrderDto.OrderDate,
                OrderLines = updateOrderDto.OrderLines
            };

            // Send command through MediatR
            await mediator.Send(command);

            return Results.NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return Results.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid argument when updating order {OrderId}", id);
            return Results.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Business rule violation when updating order {OrderId}", id);
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order {OrderId}", id);
            return Results.Problem("Internal server error", statusCode: 500);
        }
    }

    private static async Task<IResult> DeleteOrder(
        Guid id,
        IMediator mediator,
        ILogger<Program> logger)
    {
        try
        {
            var command = new DeleteOrderCommand { Id = id };
            var deleted = await mediator.Send(command);

            if (!deleted)
            {
                return Results.NotFound($"Order with ID {id} not found");
            }

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order {OrderId}", id);
            return Results.Problem("Internal server error", statusCode: 500);
        }
    }
}
