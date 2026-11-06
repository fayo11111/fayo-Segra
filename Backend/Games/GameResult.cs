using System.Text.Json;

namespace Segra.Backend.Games
{
    /// <summary>
    /// Fork: end-of-game score shown on session cards. Integrations report the latest state while
    /// the game runs; the session's metadata takes it when written (matched by game exe).
    /// </summary>
    public record GameResult(string? Outcome, int TeamScore, int EnemyScore, string? Detail)
    {
        private sealed record Entry(string? ExePath, GameResult Result);
        private static Entry? _latest;

        public static void Report(string? exePath, GameResult result) => _latest = new(exePath, result);

        public static GameResult? Take(string? exePath)
        {
            Entry? latest = _latest;
            if (latest == null || !string.Equals(latest.ExePath, exePath, StringComparison.OrdinalIgnoreCase))
                return null;
            _latest = null;
            return latest.Result;
        }

        // League live client allgamedata: team kills, GameEnd result, "Champion · K/D/A".
        public static void ReportLeague(string? exePath, JsonElement root, JsonElement player)
        {
            string? team = player.GetProperty("team").GetString();
            int teamKills = 0, enemyKills = 0;
            foreach (JsonElement p in root.GetProperty("allPlayers").EnumerateArray())
            {
                int kills = p.GetProperty("scores").GetProperty("kills").GetInt32();
                if (p.GetProperty("team").GetString() == team) teamKills += kills; else enemyKills += kills;
            }
            string? outcome = root.GetProperty("events").GetProperty("Events").EnumerateArray()
                .Where(e => e.GetProperty("EventName").GetString() == "GameEnd")
                .Select(e => e.GetProperty("Result").GetString() == "Win" ? "Win" : "Loss")
                .FirstOrDefault();
            JsonElement s = player.GetProperty("scores");
            Report(exePath, new(outcome, teamKills, enemyKills,
                $"{player.GetProperty("championName").GetString()} · {s.GetProperty("kills")}/{s.GetProperty("deaths")}/{s.GetProperty("assists")}"));
        }

        // CS2 game state integration payload: round score and K/D/A of the local player.
        public static void ReportCs2(string? exePath, string body)
        {
            using JsonDocument doc = JsonDocument.Parse(body);
            JsonElement r = doc.RootElement;
            if (!r.TryGetProperty("map", out JsonElement map) || !r.TryGetProperty("player", out JsonElement p) ||
                !p.TryGetProperty("team", out JsonElement team) || !p.TryGetProperty("match_stats", out JsonElement s) ||
                p.GetProperty("steamid").GetString() != r.GetProperty("provider").GetProperty("steamid").GetString())
                return;
            bool ct = team.GetString() == "CT";
            int mine = map.GetProperty(ct ? "team_ct" : "team_t").GetProperty("score").GetInt32();
            int theirs = map.GetProperty(ct ? "team_t" : "team_ct").GetProperty("score").GetInt32();
            string? outcome = map.GetProperty("phase").GetString() != "gameover" ? null
                : mine > theirs ? "Win" : mine < theirs ? "Loss" : "Draw";
            Report(exePath, new(outcome, mine, theirs, $"{s.GetProperty("kills")}/{s.GetProperty("deaths")}/{s.GetProperty("assists")}"));
        }
    }
}
