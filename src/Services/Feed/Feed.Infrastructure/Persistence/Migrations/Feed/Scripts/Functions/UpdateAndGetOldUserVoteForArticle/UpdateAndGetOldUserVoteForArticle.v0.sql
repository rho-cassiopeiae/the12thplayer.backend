SET search_path TO feed;

CREATE OR REPLACE FUNCTION update_and_get_old_user_vote_for_article (
    user_id BIGINT,
    article_id INT,
    vote SMALLINT,
    OUT old_vote SMALLINT
)
LANGUAGE PLPGSQL
AS $$
BEGIN
    SET search_path TO feed;

    INSERT INTO "UserVotes" ("UserId", "ArticleId", "ArticleVote")
    VALUES (user_id, article_id, vote)
    ON CONFLICT ON CONSTRAINT "PK_UserVotes" DO
        UPDATE
        SET
            "ArticleVote" =
                CASE
                    WHEN "UserVotes"."ArticleVote" IS NULL OR "UserVotes"."ArticleVote" != vote
                        THEN vote
                    ELSE
                        NULL
                END,
            "OldVote" = "UserVotes"."ArticleVote"
    RETURNING "OldVote"
    INTO old_vote;
END;
$$