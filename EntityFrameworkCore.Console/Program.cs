﻿//First we need an instance of context
using EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;

using var context = new FootballLeagueDbContext();
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
        .OrderByDescending(q => q.TeamId)
        .FirstOrDefaultAsync();
    // or
    var maxBy = context.Teams.MaxBy(q => q.TeamId);

    // Getting the record with a Minimum value
    var minByDescendingOrder = await context.Teams
        .OrderBy(q => q.TeamId)
        .FirstOrDefaultAsync();
    // or
    var minBy = context.Teams.MinBy(q => q.TeamId);
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
        Console.WriteLine(group.Sum(q => q.TeamId));
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

    var numberOfTeamsWithConditions = await context.Teams.CountAsync(q => q.TeamId == 1);
    Console.WriteLine($"Number of Teams with Conditions: {numberOfTeamsWithConditions}");

    // Max
    var maxTeams = await context.Teams.MaxAsync(q => q.TeamId);
    // Min
    var minTeams = await context.Teams.MinAsync(q => q.TeamId);
    // Average
    var avgTeams = await context.Teams.AverageAsync(q => q.TeamId);
    // Sum
    var sumTeams = await context.Teams.SumAsync(q => q.TeamId);
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
    var teamFirstWithCondition = await context.Teams.FirstAsync(team => team.TeamId == 1);
    if (teamFirstWithCondition != null)
    {
        Console.WriteLine(teamFirstWithCondition.Name);
    }
    var teamFirsOrDefaulttWithCondition = await context.Teams.FirstOrDefaultAsync(team => team.TeamId == 1);
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
    var teamSingleWithCondition = await context.Teams.SingleAsync(team => team.TeamId == 2);
    if (teamSingleWithCondition != null)
    {
        Console.WriteLine(teamSingleWithCondition.Name);
    }
    var singleOrDefault = await context.Teams.SingleOrDefaultAsync(team => team.TeamId == 2);
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