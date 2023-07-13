# Idex.Net

Idex.Net is a beautiful .Net wrapper for the Idex API. This library provides a convenient way to interact with the Idex API by offering a comprehensive set of features. With Idex.Net, you can seamlessly access the REST API using clear and readable objects. It empowers you to perform various actions such as reading market information, placing and managing orders, and retrieving balances and funds.

## Features

- **REST API Integration**: Idex.Net seamlessly integrates with the Idex API, allowing you to interact with the platform effortlessly.
- **Clear and Readable Objects**: The library provides easy-to-understand objects that enhance code readability and maintainability.
- **Market Information**: Retrieve comprehensive market information to make informed decisions.
- **Order Management**: Place and manage orders conveniently using intuitive methods.
- **Balance and Fund Retrieval**: Access and retrieve balances and funds for efficient portfolio management.

## Getting Started

To get started with Idex.Net, follow these steps:

1. Install the library via NuGet or include the necessary source files in your project.
2. Obtain your API key from the Idex platform.
3. Instantiate the Idex.Net client with your API key.
4. Begin utilizing the library's rich functionality to interact with the Idex API.

```csharp
// Instantiate the Idex.Net client with your API key
var idexClient = new IdexClient("YOUR_API_KEY");

// Example: Reading balances
var balances = await idexClient.GetBalances(CancellationToken.None);

foreach (var balance in balances)
{
    Console.WriteLine($"{balance.Currency}: {balance.Amount}");
}
// Example: Placing an order
var order = new Order
{
    Symbol = "ETH_USDT",
    Side = OrderSide.Buy,
    Quantity = 1,
    Price = 3500
};

var orderId = idexClient.CreateOrder(order, CancellationToken.None);
Console.WriteLine($"Order placed. Order ID: {orderId}");
```
## Documentation

For detailed information on how to use Idex API, refer to the [documentation]([https://github.com/your-username/your-repo/wiki](https://api-docs-v3.idex.io/#introduction)).

## Contributing

Contributions to Idex.Net are welcome! If you encounter any issues or have suggestions for improvements, please open an issue or submit a pull request on the [GitHub repository]([https://github.com/your-username/your-repo](https://github.com/olehrechukh/Idex4Net/pulls)).

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT). Feel free to use, modify, and distribute this library in accordance with the terms of the license.

## Acknowledgments

We would like to express our gratitude to the Idex team for providing the API and making this wrapper possible.
