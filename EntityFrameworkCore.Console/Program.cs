using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;

// First we need an instance of context
using var context = new FootballLeagueDbContext();

// Use to automatically apply all outstanding migrations
// Carefully consider before using this approach in production.
//context.Database.MigrateAsync();

// For SQLite Users to see where the Database file gets created
//Console.WriteLine(context.DbPath);

#region Read Queries
// Select all teams
//await GetAllTeams();
//await GetAllTeamsQuerySyntax();


// Select One team
//await GetOneTeam();

// Select all record that meet a condition
//await GetFilteredTeams();

// Aggregate Methods
//await AggregateMethods();


// Grouping and Aggregating
// GroupByMethod();

// Ordering
// OrderByMethods();

// Skip and Take - Great for Paging
// await SkipAndTake();

// Select and Projections - more precise queries
// await ProjectionsAndSelect();

// No Tracking - EF Core tracks objects that are returned by queries. This is less useful in
// disconnected applications like APIs and Web apps
// await NoTracking();

// IQueryables vs List Types
// await ListVsQueryable();
#endregion

#region Write Queries
// Inserting Data 
/* INSERT INTO Coaches (colos) VALUES (values) */

// Simple Insert
// await InsertOneRecord();

// Loop Insert
//await InsertWithLoop();

// Batch Insert
//await InsertRange();

// Update Operations
//await UpdateWithTracking();
//await UpdateNoTracking();

// Delete Operations
//await DeleteRecord();

// Excute Delete ef >=7
//await ExcuteDelete();


// Excute Update ef >=7
//await ExcuteUpdate();

#endregion
#region Related Data
// Insert record with FK
//var match = new Match
//{
//    AwayTeamId = 1,
//    HomeTeamId = 2,
//    HomeTeamScore = 0,
//    AwayTeamScore = 0,
//    Date = new DateTime(2024, 10, 1),
//    TicketPrice = 20,
//};
//await context.AddAsync(match);
//await context.SaveChangesAsync();

//var match1 = new Match
//{
//    AwayTeamId = 10,
//    HomeTeamId = 0,
//    HomeTeamScore = 0,
//    AwayTeamScore = 0,
//    Date = new DateTime(2024, 10, 1),
//    TicketPrice = 20,
//};
//await context.AddAsync(match1);
//await context.SaveChangesAsync();

// Insert Parent/Child

//var team = new Team
//{
//    Name = "New Team",
//    Coach = new Coach
//    {
//        Name = "Johnson"
//    }

//};
//await context.AddAsync(team);
//await context.SaveChangesAsync();

// Insert Parent with Children
var league = new League
{
    Name = "Serie A",
    Teams = new List<Team>
    {
        new Team
        {
            Name = "Juventus",
            Coach = new Coach
            {
                Name = "Juve Coach"
            }
        },
        new Team
        {
            Name = "Ac Milan",
            Coach = new Coach
            {
                Name = "Milan Coach"
            }
        },
        new Team
        {
            Name = "AS Roma",
            Coach = new Coach
            {
                Name = "Roma Coach"
            }
        },
    }
};
await context.AddAsync(league);
await context.SaveChangesAsync();

#endregion


async Task ExcuteDelete()
{
    var coaches = await context.Coaches.Where(q => q.Name == "No Tracking").ExecuteDeleteAsync(); // Immediately no save change necessary
}
async Task ExcuteUpdate()
{
    var coaches = await context.Coaches.Where(q => q.Name == "Jose Mourinho").ExecuteUpdateAsync(set => set
            .SetProperty(prop => prop.Name, "Peppe Guardiola")
    //.SetProperty(prop => prop.CreatedDate, DateTime.Now)
    ); // Immediately no save change necessary
}

async Task DeleteRecord()
{
    /*DELETE FROMCoaches WHERE Id = 1*/
    var coach = await context.Coaches.FindAsync(5);
    context.Remove(coach);
    //context.Entry(coach).State = EntityState.Deleted; // Same as Above
    await context.SaveChangesAsync();
}



async Task UpdateWithTracking()
{
    var coach = await context.Coaches.FindAsync(3);
    coach.Name = "Trevoir Wiliams";
    coach.CreatedDate = DateTime.Now;
    await context.SaveChangesAsync();
}
async Task UpdateNoTracking()
{
    var coach1 = await context.Coaches
        .AsNoTracking()
        .FirstOrDefaultAsync(q => q.Id == 4);
    coach1.Name = "No Tracking";

    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    context.Update(coach1); // so force to track again
                            //context.Entry(coach1).State = EntityState.Modified; // same as above
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    await context.SaveChangesAsync();
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
}



async Task InsertOneRecord()
{

    var newCoach = new Coach
    {
        Name = "Carletto Mazzone",
        CreatedDate = DateTime.Now,
    };
    await context.Coaches.AddAsync(newCoach);
    await context.SaveChangesAsync();
}

async Task InsertWithLoop()
{
    var newCoach = new Coach
    {
        Name = "Carletto Mazzone",
        CreatedDate = DateTime.Now,
    };

    var newCoach1 = new Coach
    {
        Name = "Carlo Ancelotti",
        CreatedDate = DateTime.Now,
    };
    List<Coach> coaches = new List<Coach>()
    {
        newCoach,
        newCoach1,
    };
    foreach (var coache in coaches)
    {
        await context.AddAsync(coache);
    }
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    await context.SaveChangesAsync();// pass only at end 
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    foreach (var coache in coaches)
    {
        Console.WriteLine($"{coache.Id} - {coache.Name}");
    }
}

async Task InsertRange()
{
    var newCoach = new Coach
    {
        Name = "Carletto Mazzone",
        CreatedDate = DateTime.Now,
    };

    var newCoach1 = new Coach
    {
        Name = "Carlo Ancelotti",
        CreatedDate = DateTime.Now,
    };
    List<Coach> coaches = new List<Coach>()
    {
        newCoach,
        newCoach1,
    };

    await context.Coaches.AddRangeAsync(coaches);
    await context.SaveChangesAsync();
}


async Task ListVsQueryable()
{
    Console.WriteLine("Enter '1' for Team with Id 1 or '2' for teams that contain 'F.C.'");
    var option = Convert.ToInt32(Console.ReadLine());
    List<Team> teamsAsList = new List<Team>();

    // After executing to ListAsync, the records are loaded into memory. Any operation is the done in memory
    teamsAsList = await context.Teams.ToListAsync();
    if (option == 1)
    {
        teamsAsList = teamsAsList.Where(q => q.Id == 1).ToList();
    }
    else if (option == 2)
    {
        teamsAsList = teamsAsList.Where(q => q.Name.Contains("F.C.")).ToList();
    }

    foreach (var t in teamsAsList)
    {
        Console.WriteLine(t.Name);
    }
    // Records stay as IQuerable until the ToListAsync is executed, then the final query is performed.
    var teamsAsQueryable = context.Teams.AsQueryable();
    if (option == 1)
    {
        teamsAsQueryable = teamsAsQueryable.Where(q => q.Id == 1);
    }
    else if (option == 2)
    {
        teamsAsQueryable = teamsAsQueryable.Where(q => q.Name.Contains("F.C."));
    }

    // Actual Query execution
    teamsAsList = await teamsAsQueryable.ToListAsync();
    foreach (var t in teamsAsList)
    {
        Console.WriteLine(t.Name);
    }
}

async Task NoTracking()
{
    var teams = await context.Teams
        .AsNoTracking()
        .ToListAsync();

    foreach (var t in teams)
    {
        Console.WriteLine(t.Name);
    }
}

async Task ProjectionsAndSelect()
{
    var teamNames = await context.Teams
        //.Select(q => q.Name) //One field
        .Select(q => new TeamInfo { Name = q.Name, Id = q.Id }) // more than one
        .ToListAsync();

    foreach (var name in teamNames)
    {
        Console.WriteLine($"{name.Name} - {name.Id}");
    }
}

async Task SkipAndTake()
{
    var recordCount = 3;
    var page = 0;
    var next = true;
    while (next)
    {
        var teams = await context.Teams.Skip(page * recordCount).Take(recordCount).ToListAsync();
        foreach (var item in teams)
        {
            Console.WriteLine(item.Name);
        }
        Console.WriteLine("Enter 'true' for the next set of records, 'false' to exit");
        next = Convert.ToBoolean(Console.ReadLine());

        if (!next) break;
        page += 1;
    }
}

async Task OrderByMethods()
{

    var orderedTeams = await context.Teams
        .OrderBy(q => q.Name)
        .ToListAsync();

    foreach (var item in orderedTeams)
    {
        Console.WriteLine(item.Name);
    }

    var descOrderedTeams = await context.Teams
        .OrderByDescending(q => q.Name)
        .ToListAsync();

    foreach (var item in descOrderedTeams)
    {
        Console.WriteLine(item.Name);
    }
    // Getting the record with a Maximum value
    var maxByDescendingOrder = await context.Teams
        .OrderByDescending(q => q.Id)
        .FirstOrDefaultAsync();
    // or
    var maxBy = context.Teams.MaxBy(q => q.Id);

    // Getting the record with a Minimum value
    var minByDescendingOrder = await context.Teams
        .OrderBy(q => q.Id)
        .FirstOrDefaultAsync();
    // or
    var minBy = context.Teams.MinBy(q => q.Id);
}

void GroupByMethod()
{
    var groupedTeams = context.Teams
        //.Where(q =>  q.Name == "") // Translate to a WHERE clause
        .GroupBy(q => new { q.CreatedDate.Date }) // new only if more of groupby column
                                                  //.Where()// Translate to a HAVING clause
                                                  //.ToList() // Use the executing method to load the results into memory before processing
        ;

    // EF core can iterate through records on demand. Here, there is no executing method, but EF Core is bringing back records per iteration.
    // This is convenient, but dangerous when you have several operation to complete per iteration.
    // It is generally better to execute with ToList() and then operate on whatever is returned to memory.
    foreach (var group in groupedTeams)
    {
        Console.WriteLine(group.Key); // Key rappresent the key of the group by
        Console.WriteLine(group.Sum(q => q.Id));
        foreach (var team in group)
        {
            Console.WriteLine(team.Name);
        }
    }
}

async Task AggregateMethods()
{
    // Count
    var numberOfTeams = await context.Teams.CountAsync();
    Console.WriteLine($"Number of Teams: {numberOfTeams}");

    var numberOfTeamsWithConditions = await context.Teams.CountAsync(q => q.Id == 1);
    Console.WriteLine($"Number of Teams with Conditions: {numberOfTeamsWithConditions}");

    // Max
    var maxTeams = await context.Teams.MaxAsync(q => q.Id);
    // Min
    var minTeams = await context.Teams.MinAsync(q => q.Id);
    // Average
    var avgTeams = await context.Teams.AverageAsync(q => q.Id);
    // Sum
    var sumTeams = await context.Teams.SumAsync(q => q.Id);
}

async Task GetFilteredTeams()
{
    Console.WriteLine("Entered Search Team");
    var searchTerm = Console.ReadLine();
    var teamsFiltered = await context.Teams.Where(q => q.Name == searchTerm)
        .ToListAsync();

    foreach (var item in teamsFiltered)
    {
        Console.WriteLine(item.Name);
    }

    //var partialMatches = await context.Teams.Where(q => q.Name.Contains(searchTerm)).ToListAsync();
    var partialMatches = await context.Teams.Where(q => EF.Functions.Like(q.Name, $"%{searchTerm}%")).ToListAsync();
    foreach (var item in partialMatches)
    {
        Console.WriteLine(item.Name);
    }
}

async Task GetAllTeams()
{
    // SELECT * FROM teams
    var teams = await context.Teams.ToListAsync();

    foreach (var t in teams)
    {
        Console.WriteLine(t.Name);
    }
}

async Task GetOneTeam()
{
    // Selecting a single record - First one in list
    var teamFirst = await context.Coaches.FirstAsync();
    if (teamFirst != null)
    {
        Console.WriteLine(teamFirst.Name);
    }
    var teamFirstOrDefault = await context.Coaches.FirstOrDefaultAsync();
    if (teamFirstOrDefault != null)
    {
        Console.WriteLine(teamFirstOrDefault.Name);
    }


    // Selecting a single record - First one in list that meets a condition
    var teamFirstWithCondition = await context.Teams.FirstAsync(team => team.Id == 1);
    if (teamFirstWithCondition != null)
    {
        Console.WriteLine(teamFirstWithCondition.Name);
    }
    var teamFirsOrDefaulttWithCondition = await context.Teams.FirstOrDefaultAsync(team => team.Id == 1);
    if (teamFirsOrDefaulttWithCondition != null)
    {
        Console.WriteLine(teamFirsOrDefaulttWithCondition.Name);
    }

    // Selecting a single record - Only one record should be returned, or an exception will be thrown
    var teamSingle = await context.Teams.SingleAsync();
    if (teamSingle != null)
    {
        Console.WriteLine(teamSingle.Name);
    }
    var teamSingleWithCondition = await context.Teams.SingleAsync(team => team.Id == 2);
    if (teamSingleWithCondition != null)
    {
        Console.WriteLine(teamSingleWithCondition.Name);
    }
    var singleOrDefault = await context.Teams.SingleOrDefaultAsync(team => team.Id == 2);
    if (singleOrDefault != null)
    {
        Console.WriteLine(singleOrDefault.Name);
    }

    // selecting based on Id
    var teamBasedOnId = await context.Teams.FindAsync(3);
    if (teamBasedOnId != null)
    {
        Console.WriteLine(teamBasedOnId.Name);
    }
}

async Task GetAllTeamsQuerySyntax()
{
    Console.WriteLine("Entered Search Team");
    var searchTerm = Console.ReadLine();
    var teams = await (from team in context.Teams
                       where EF.Functions.Like(team.Name, $"%{searchTerm}%")
                       select team)
                       .ToListAsync();
    foreach (var t in teams)
    {
        Console.WriteLine(t.Name);
    }
}

class TeamInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
}