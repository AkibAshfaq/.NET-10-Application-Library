Ecommarce.DAL/
├── Entities/                          # Plain POCOs — properties only, no logic, no data annotations
│   ├── Product.cs                     # Id, Name, Price, StockQuantity, CategoryId + Category nav property
│   ├── Order.cs                       # Id, CustomerId + Customer nav, List<OrderItem>, TotalAmount, Status enum
│   └── Customer.cs                    # Id, FullName, Email, PhoneNumber, List<Address>, List<Order>
│
├── Configurations/                    # One class per entity, implements IEntityTypeConfiguration<T>
│   ├── ProductConfiguration.cs        # HasKey, required/maxlength on Name, decimal(18,2) on Price, FK to Category, index on Name
│   ├── OrderConfiguration.cs          # HasKey, FK to Customer, one-to-many to OrderItems, enum-to-string conversion for Status
│   └── CustomerConfiguration.cs       # HasKey, unique index on Email, one-to-many to Addresses/Orders
│
├── Context/
│   └── EcommerceDbContext.cs          # DbSet<T> per entity + OnModelCreating calling ApplyConfigurationsFromAssembly
│
├── Migrations/                        # Don't write by hand — generated via `dotnet ef migrations add`
│
├── Repositories/
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs      # Generic contract: GetByIdAsync, GetAllAsync, AddAsync, Update, Delete
│   │   └── IProductRepository.cs      # Extends IGenericRepository<Product>, adds GetByCategoryAsync, SearchByNameAsync
│   ├── GenericRepository.cs           # Implements IGenericRepository<T> using DbContext.Set<T>()
│   └── ProductRepository.cs           # Extends GenericRepository<Product>, implements the extra query methods
│
├── UnitOfWork/
│   ├── IUnitOfWork.cs                 # Exposes one property per repository + SaveChangesAsync()
│   └── UnitOfWork.cs                  # Constructs each repository with the shared DbContext, wraps SaveChangesAsync
│
├── Seed/
│   └── DbInitializer.cs               # Static SeedAsync(DbContext) method — check if data exists, insert sample rows if not
│
└── Extensions/
    └── ServiceCollectionExtensions.cs # AddDataAccessLayer(IServiceCollection, IConfiguration): registers DbContext (UseSqlServer),
                                        # registers repositories + UnitOfWork as Scoped — called once from Application's Program.cs