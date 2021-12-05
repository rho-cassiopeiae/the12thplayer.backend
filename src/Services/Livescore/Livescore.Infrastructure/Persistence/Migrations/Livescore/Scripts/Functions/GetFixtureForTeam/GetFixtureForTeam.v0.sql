SET search_path TO livescore;

CREATE OR REPLACE FUNCTION get_fixture_for_team (
    fixture_id BIGINT,
    team_id BIGINT
) RETURNS table (
    "Id" BIGINT,
    "HomeStatus" BOOLEAN,
    "StartTime" BIGINT,
    "Status" TEXT,
    "GameTime" JSONB,
    "Score" JSONB,
    "RefereeName" TEXT,
    "Colors" JSONB,
    "Lineups" JSONB,
    "Events" JSONB,
    "Stats" JSONB,
    "LeagueName" TEXT,
    "LeagueLogoUrl" TEXT,
    "OpponentTeamId" BIGINT,
    "OpponentTeamName" TEXT,
    "OpponentTeamLogoUrl" TEXT,
    "VenueName" TEXT,
    "VenueImageUrl" TEXT
)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO livescore;

    RETURN QUERY
        SELECT
            f."Id", f."HomeStatus", f."StartTime", f."Status", f."GameTime", f."Score",
            f."RefereeName", f."Colors", f."Lineups", f."Events", f."Stats",
            l."Name", l."LogoUrl",
            t."Id", t."Name", t."LogoUrl",
            v."Name", v."ImageUrl"
        FROM
            "Fixtures" f
                LEFT JOIN
            "Season" s
                ON (f."SeasonId" = s."Id")
                    LEFT JOIN
                "Leagues" l
                    ON (s."LeagueId" = l."Id")
                INNER JOIN
            "Teams" t
                ON (f."OpponentTeamId" = t."Id")
                LEFT JOIN
            "Venues" v
                ON (f."VenueId" = v."Id")
        WHERE f."Id" = fixture_id AND f."TeamId" = team_id;
END;
$$