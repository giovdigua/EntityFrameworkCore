//First we need an instance of context
using EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;

using var context = new FootballLeagueDbContext();

// Select all teams
//GetAllTeams();

// Selecting a single record - First one in list
//var teamOne = await context.Coaches.FirstOrDefaultAsync();
//var teamOne = await context.Teams.FirstOrDefaultAsync();

// Selecting a single record - First one in list that meets a condition
//var teamTwo = await context.Teams.FirstAsync(team => team.TeamId == 1);
//var teamTwo = await context.Teams.FirstOrDefaultAsync(team => team.TeamId == 1);

// Selecting a single record - Only one record should be returned
//var teamsThree = await context.Teams.SingleAsync();
//var teamThree = await context.Teams.SingleAsync(team => team.TeamId == 2);
//var teamFour = await context.Teams.SingleOrDefaultAsync(team => team.TeamId == 2);

// selecting based on Id
var teamBasedOnId = await context.Teams.FindAsync(3);
if (teamBasedOnId != null )
{
    Console.WriteLine(teamBasedOnId.Name);
}
void GetAllTeams()
{
    // SELECT * FROM teams
    var teams = context.Teams.ToList();

    foreach (var t in teams)
    {
        Console.WriteLine(t.Name);
    }
}

