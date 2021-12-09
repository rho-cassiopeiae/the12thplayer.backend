SET search_path TO livescore;

CREATE OR REPLACE PROCEDURE insert_or_ignore_user_vote_multi (
	user_ids bigint[],
	fixture_ids bigint[],
	team_ids bigint[],
	player_ratings jsonb[],
	live_commentary_votes jsonb[],
	video_reaction_votes jsonb[]
)
LANGUAGE PLPGSQL
AS $$
BEGIN
	SET search_path TO livescore;

	INSERT INTO "UserVotes" ("UserId", "FixtureId", "TeamId", "FixtureParticipantKeyToRating", "LiveCommentaryAuthorIdToVote", "VideoReactionAuthorIdToVote")
	SELECT * FROM UNNEST (user_ids, fixture_ids, team_ids, player_ratings, live_commentary_votes, video_reaction_votes)
	ON CONFLICT DO NOTHING;
END;
$$