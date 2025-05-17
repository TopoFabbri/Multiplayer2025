using Server;

Application app = new();

try
{
    app.Run(int.Parse(args[0]));
}
finally
{
    Console.ReadKey();
}