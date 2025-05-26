# Docker Setup for EFTest

## Quick Start

1. **Start MongoDB and Mongo Express**:
   ```bash
   docker compose up -d
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Access services**:
   - API: http://localhost:5201/api/orders
   - Mongo Express: http://localhost:8081

## Rider Integration

1. Right-click on `docker-compose.yml` in the project explorer
2. Select "Run docker-compose.yml" 
3. Use Rider's Docker tool window to manage containers
4. Run the EFTest project normally

## Services

- **MongoDB**: Latest version (8.0.9+), Port 27017, Database: OrdersDb
- **Mongo Express**: Latest version, Port 8081, Web admin interface

## Commands

```bash
# Start services
docker compose up -d

# Stop services  
docker compose down

# View logs
docker compose logs

# Reset data
docker compose down -v
docker compose up -d
```
