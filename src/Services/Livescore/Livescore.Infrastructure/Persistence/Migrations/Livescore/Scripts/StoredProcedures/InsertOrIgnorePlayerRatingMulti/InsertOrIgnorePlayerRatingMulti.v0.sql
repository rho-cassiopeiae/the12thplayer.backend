SET search_path TO livescore;

CREATE OR REPLACE PROCEDURE insert_or_ignore_player_rating_multi (
	fixture_ids bigint[],
	team_ids bigint[],
	participant_keys text[],
	total_ratings int[],
	total_voters int[]
)
LANGUAGE PLPGSQL
AS $$
BEGIN
	SET search_path TO livescore;

	INSERT INTO "PlayerRatings" ("FixtureId", "TeamId", "ParticipantKey", "TotalRating", "TotalVoters")
	SELECT * FROM UNNEST (fixture_ids, team_ids, participant_keys, total_ratings, total_voters)
	ON CONFLICT DO NOTHING;
END;
$$