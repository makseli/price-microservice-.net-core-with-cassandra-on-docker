using Cassandra;
using Grpc.Core;
using ProductService;

public class ProductService : Product.ProductBase {
    private readonly ILogger<ProductService> _logger;
    private readonly ISession _session;

    public ProductService(ILogger<ProductService> logger) {
        _logger = logger;
        var cluster = Cluster.Builder().AddContactPoint("cassandra").Build();
        _session = cluster.Connect("productdb");
    }

    public override Task<ProductReply> AddProduct(ProductRequest request, ServerCallContext context) {
        var cql = "INSERT INTO products (id, name, website_id, price_cash, price_credit_card, created_at, updated_at) VALUES (uuid(), ?, ?, ?, ?, toTimestamp(now()), toTimestamp(now()))";
        var statement = _session.Prepare(cql);
        var boundStatement = statement.Bind(request.Name, request.WebsiteId, request.PriceCash, request.PriceCreditCard);
        _session.Execute(boundStatement);

        return Task.FromResult(new ProductReply {
            Message = "Product added successfully"
        });
    }
}
