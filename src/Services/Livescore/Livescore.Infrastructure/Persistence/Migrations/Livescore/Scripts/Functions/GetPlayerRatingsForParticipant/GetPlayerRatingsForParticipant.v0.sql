SET search_path TO livescore;

CREATE OR REPLACE FUNCTION get_player_ratings_for_participant (
    team_id BIGINT,
    participant_keys TEXT[]
)
RETURNS table (
    "OpponentTeamName" TEXT,
    "OpponentTeamLogoUrl" TEXT,
    "FixtureStartTime" BIGINT,
    "FixtureHomeStatus" BOOLEAN,
    "HomeTeamScore" SMALLINT,
    "GuestTeamScore" SMALLINT,
    "TotalRating" INTEGER,
    "TotalVoters" INTEGER
)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO livescore;

    RETURN QUERY
        SELECT
            t."Name", t."LogoUrl", f."StartTime", f."HomeStatus",
            (f."Score" ->> 'LocalTeam')::SMALLINT, (f."Score" ->> 'VisitorTeam')::SMALLINT,
            pr."TotalRating", pr."TotalVoters"
        FROM
            "PlayerRatings" AS pr
                INNER JOIN
            "Fixtures" AS f ON (pr."FixtureId" = f."Id" AND pr."TeamId" = f."TeamId")
                INNER JOIN
            "Teams" AS t ON (f."OpponentTeamId" = t."Id")
                INNER JOIN
            "Season" AS s ON (f."SeasonId" = s."Id")
        WHERE
            pr."TeamId" = team_id AND
            pr."ParticipantKey" = ANY(participant_keys) AND
            s."IsCurrent"
        ORDER BY f."StartTime" DESC;
END;
$$