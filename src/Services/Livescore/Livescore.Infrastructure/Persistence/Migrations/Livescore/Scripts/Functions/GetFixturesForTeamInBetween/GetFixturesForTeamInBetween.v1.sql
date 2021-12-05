SET search_path TO livescore;

CREATE OR REPLACE FUNCTION get_fixtures_for_team_in_between (
    team_id BIGINT,
    start_time BIGINT,
    end_time BIGINT
) RETURNS table (
    "Id" BIGINT,
    "HomeStatus" BOOLEAN,
    "StartTime" BIGINT,
    "Status" TEXT,
    "GameTime" JSONB,
    "Score" JSONB,
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
        WHERE
            f."TeamId" = team_id AND
            f."StartTime" > start_time AND
            f."StartTime" < end_time;
END;
$$