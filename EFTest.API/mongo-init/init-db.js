// MongoDB initialization script
// This script creates the OrdersDb database and a user for the application

db = db.getSiblingDB('OrdersDb');

// Create a user for the application
db.createUser({
  user: 'eftest_user',
  pwd: 'eftest_password',
  roles: [
    {
      role: 'readWrite',
      db: 'OrdersDb'
    }
  ]
});

// Create collections (optional, EF Core will create them automatically)
db.createCollection('orders');
db.createCollection('orderlines');

print('Database OrdersDb initialized successfully');
