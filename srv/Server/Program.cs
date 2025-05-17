using Server;

Application app = new();

try
{
    app.Run(int.Parse(args[0]));
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

Console.ReadKey();