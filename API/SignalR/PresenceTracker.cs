using System;

namespace API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string /* Username */, List<string> /* Connection Ids */> OnlineUsers = [];

    public Task UserConnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username)) // exists
            {
                OnlineUsers[username].Add(connectionId);
            }
            else // does not exist
            {
                OnlineUsers.Add(username, [connectionId]);
            }
        }

        return Task.CompletedTask;
    }

    public Task UserDisconnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask; // is not here

            OnlineUsers[username].Remove(connectionId); // remove this Id

            if (OnlineUsers[username].Count == 0) // user has no connections in list
            {
                OnlineUsers.Remove(username); // remove the user from the dict
            }
        }

        return Task.CompletedTask;
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            // just get an ordered list of usernames
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    public static Task<List<string>> GetConnectionsForUser(string username)
    {
        List<string> connectionIds;

        if (OnlineUsers.TryGetValue(username, out var connections))
        {
            lock (connections)
            {
                connectionIds = [.. connections];
            }
        }
        else
        {
            connectionIds = [];
        }

        return Task.FromResult(connectionIds);
    }
}
