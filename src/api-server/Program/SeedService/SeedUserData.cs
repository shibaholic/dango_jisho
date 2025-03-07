using Application.UseCaseCommands.Auth;
using MediatR;

namespace Program.SeedService;

public static class SeedUserData
{
    public static async Task Run(IServiceScope serviceScope)
    {
        Console.WriteLine("Seeding with User data...");
        
        // var userCrud = serviceScope.ServiceProvider.GetService<ICrudService<User,User>>();
        var mediator = serviceScope.ServiceProvider.GetService<IMediator>();

        if (mediator == null)
        {
            Console.WriteLine("  Error while getting mediator.");
            System.Environment.Exit(1);
        }

        var createUserRequest = new CreateUserRequest
        {
            UserId = new Guid("faeb2480-fbdc-4921-868b-83bd93324099"),
            Username = "qwe",
            Password = "asd"
        };
        
        var result = await mediator.Send(createUserRequest, CancellationToken.None);
        
        if (!result.Successful)
        {
            Console.WriteLine("  Error while seeding user.");
            System.Environment.Exit(1);
        }
        
        Console.WriteLine("Successfully seeded user.\n");
    }
}