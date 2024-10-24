namespace Test.ProductApi.Controllers;

public class ProductControllerTest
{
    private readonly AutoMocker _autoMocker;
    private readonly Mock<IProductService> _productService;
    private readonly Mock<INotifierService> _notifierService;
    private readonly ProductController _productController;

    public ProductControllerTest()
    {
        _autoMocker = new AutoMocker();
        _productService = _autoMocker.GetMock<IProductService>();
        _notifierService = _autoMocker.GetMock<INotifierService>();
        _productController = _autoMocker.CreateInstance<ProductController>();
    }

    [Fact]
    public async void GetAll_ShouldReturnAll_WhenThereAreDataIntoDatabase()
    {
        // Arrange
        var products = ProductDtoFaker.ProductDtoList();

        _productService.Setup(x => x.GetAll()).ReturnsAsync(products);

        // Act
        var response = await _productController.GetAll() as ObjectResult;
        var result = response.Value as ResponseDto;

        // Assert
        response.Should().NotBeNull();
        response.Value.Should().NotBeNull();
        response.Value.Should().BeOfType<ResponseDto>();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);

        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Content.Should().BeOfType<List<ProductDto>>();
        result.Content.Should().BeEquivalentTo(products);
        (result.Content as List<ProductDto>).Should().HaveCount(3);

        _productService.Verify(x => x.GetAll(), Times.Once);
        _notifierService.Verify(x => x.HasMessages(), Times.Once);
    }

    [Fact]
    public async void GetAll_ShouldReturnEmpty_WhenThereAreNoDataIntoDatabase()
    {
        // Arrange
        _productService.Setup(x => x.GetAll()).ReturnsAsync([]);

        // Act
        var response = await _productController.GetAll() as ObjectResult;
        var result = response.Value as ResponseDto;

        // Assert
        response.Should().NotBeNull();
        response.Value.Should().NotBeNull();
        response.Value.Should().BeOfType<ResponseDto>();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);

        result.Should().NotBeNull();
        result.Status.Should().BeTrue();
        result.Content.Should().BeOfType<List<ProductDto>>();
        (result.Content as List<ProductDto>).Should().HaveCount(0);

        _productService.Verify(x => x.GetAll(), Times.Once);
        _notifierService.Verify(x => x.HasMessages(), Times.Once);
    }
}
