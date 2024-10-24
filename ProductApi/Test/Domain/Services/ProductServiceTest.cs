namespace Test.Domain.Services;

public class ProductServiceTest
{
    private readonly AutoMocker _autoMocker;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<INotifierService> _notifierService;
    private readonly ProductService _productService;

    public ProductServiceTest()
    {
        _autoMocker = new AutoMocker();
        _productRepository = _autoMocker.GetMock<IProductRepository>();
        _notifierService = _autoMocker.GetMock<INotifierService>();
        _productService = _autoMocker.CreateInstance<ProductService>();
    }

    [Fact]
    //nome do metodo _ o que ele deve retornar _ quando ele deve retornar
    public async Task GetAll_ShouldReturnAll_WhenThereAreDataIntoDatabase()
    {
        // Arrange
        var products = ProductFaker.ProductList();

        _productRepository.Setup(x => x.GetAll()).ReturnsAsync(products);

        // Act
        var result = await _productService.GetAll();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(products);

        _productRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenIdIsValidAndExistsInDatabase()
    {
        // Arrange
        var product = ProductFaker.ProductValid();

        _productRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductById(product.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(product);
        result.Id.Should().Be(1);
        result.Price.Should().Be(100);
        result.Name.Should().Be("Unit Testing");
        result.Description.Should().Be("Product Description");

        _productRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        _notifierService.Verify(x => x.AddLog(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-3)]
    [InlineData(-4)]
    [InlineData(0)]
    public async Task GetProductById_ShouldReturnNull_WhenIdIsInvalid(int id)
    {
        // Arrange
        // nothing to do here

        // Act
        var result = await _productService.GetProductById(id);

        // Assert
        result.Should().BeNull();

        _productRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Never);
        _notifierService.Verify(x => x.AddLog(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNull_WhenIdIsValidButDoNotExistsIntoDatabase()
    {
        // Arrange
        // declare a return or not does not matters as if you dont the repository will return null when you dont setup the method
        _productRepository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync((Product)null!);

        // Act
        var result = await _productService.GetProductById(99);

        // Assert
        result.Should().BeNull();

        _productRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        _notifierService.Verify(x => x.AddLog(It.IsAny<string>()), Times.Once);
    }
}
