namespace ShortURLGenerator.TelegramBot.Services.Identity;

public class IdentityMockService : IIdentityService
{
    private static List<ConnectionDto> _connections = new List<ConnectionDto>()
    {
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000000",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000001",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000002",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000003",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000004",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000005",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000006",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000007",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000008",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000009",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000010",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        },
        new ConnectionDto()
        {
            ConnectionId = "00000000000000000011",
            ActiveSecondsAgo = 100,
            ConnectionInfo = new ConnectionInfoDto()
            {
                Os = "Windows",
                Browser = "Chrome",
                Location = "Moscow",
                Ip = "0.0.0.0"
            }
        }
    };

    public async Task<VerificationCodeDto> GetVerificationCodeAsync(long userId)
    {
        return await Task.Run(() => new VerificationCodeDto()
        {
            Code = "000000",
            LifeTimeMinutes = 5
        });
    }

    public async Task<ConnectionsPageDto> GetConnectionsAsync(long userId, int index, int size)
    {
        return await Task.Run(() =>
        {
            var count = size <= 0 ? 0 : (int)Math.Ceiling(_connections.Count / (double)size);

            var response = new ConnectionsPageDto()
            {
                PageInfo = new PageInfoDto()
                {
                    Index = index,
                    Count = count
                }
            };

            response.Connections.AddRange(_connections.Skip(size * index).Take(size));
            return response;
        });
    }

    public async Task CloseConnectionAsync(long userId, string connectionId)
    {
        await Task.Run(() =>
        {
            _connections.RemoveAll(c => c.ConnectionId == connectionId);
        });
    }
}

