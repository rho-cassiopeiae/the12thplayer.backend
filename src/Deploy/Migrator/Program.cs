using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using IdentitySvc  = Identity.Infrastructure.Persistence;
using ProfileSvc   = Profile.Infrastructure.Persistence;
using LivescoreSvc = Livescore.Infrastructure.Persistence;

using FeedSvc = Feed.Infrastructure.Persistence;
using Feed.Domain.Aggregates.Author;
using Feed.Domain.Aggregates.Article;
using Feed.Domain.Aggregates.Comment;
using Feed.Domain.Base;

namespace Migrator {
    class Program {
        public static async Task Main(string[] args) {
            var host = Identity.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var userDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<IdentitySvc.UserDbContext>();

                userDbContext.Database.Migrate();

                var integrationEventDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<IdentitySvc.IntegrationEventDbContext>();

                integrationEventDbContext.Database.Migrate();
            }

            host = Profile.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var profileDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<ProfileSvc.ProfileDbContext>();

                profileDbContext.Database.Migrate();

                var integrationEventDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<ProfileSvc.IntegrationEventDbContext>();

                integrationEventDbContext.Database.Migrate();
            }

            host = Livescore.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var livescoreDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<LivescoreSvc.LivescoreDbContext>();

                livescoreDbContext.Database.Migrate();
            }

            host = Feed.Api.Program.CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var feedDbContext = scope
                    .ServiceProvider
                    .GetRequiredService<FeedSvc.FeedDbContext>();

                feedDbContext.Database.Migrate();
            }

            using (var scope = host.Services.CreateScope()) {
                var userId = 9877;
                var username = "user-9877";

                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                await unitOfWork.Begin();

                try {
                    var authorRepository = scope.ServiceProvider.GetRequiredService<IAuthorRepository>();
                    authorRepository.EnlistAsPartOf(unitOfWork);

                    await authorRepository.Create(new Author(
                        userId: userId,
                        email: "user-9877@email.com",
                        username: username
                    ));

                    var articleRepository = scope.ServiceProvider.GetRequiredService<IArticleRepository>();
                    int articleCount = 25;
                    var articleIds = new List<long>(articleCount);
                    for (int i = 0; i < articleCount; ++i) {
                        long articleId = await articleRepository.Create(new Article(
                            teamId: 53,
                            authorId: userId,
                            authorUsername: username,
                            postedAt: DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(i)).ToUnixTimeMilliseconds(),
                            type: ArticleType.Video,
                            title: $"video-{i}",
                            previewImageUrl: i % 2 == 0 ?
                                "https://i.ytimg.com/vi/gL45bjQvZSU/mqdefault.jpg" :
                                "https://i.ytimg.com/vi/N7yckj7hKXw/mqdefault.jpg",
                            summary: i % 2 == 0 ? $"summary-{i}" : null,
                            content: i % 2 == 0 ?
                                "https://www.youtube.com/watch?v=gL45bjQvZSU" :
                                "https://www.youtube.com/watch?v=N7yckj7hKXw",
                            rating: 0
                        ));

                        articleIds.Add(articleId);
                    }

                    var commentRepository = scope.ServiceProvider.GetRequiredService<ICommentRepository>();

                    var random = new Random();

                    for (int i = 0; i < articleCount; ++i) {
                        var articleId = articleIds[i];
                        if (random.Next(100) % 2 == 0) {
                            var commentId_1 = Ulid.NewUlid().ToString();
                            await commentRepository.Create(new Comment( // 1
                                articleId: articleId,
                                id: commentId_1,
                                rootId: commentId_1,
                                parentId: null,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-1"
                            ));
                            var commentId_1_1 = Ulid.NewUlid().ToString();
                            await commentRepository.Create(new Comment( // 1_1
                                articleId: articleId,
                                id: commentId_1_1,
                                rootId: commentId_1,
                                parentId: commentId_1,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-1-1"
                            ));
                            await commentRepository.Create(new Comment( // 1_2
                                articleId: articleId,
                                id: Ulid.NewUlid().ToString(),
                                rootId: commentId_1,
                                parentId: commentId_1,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-1-2"
                            ));
                            await commentRepository.Create(new Comment( // 1_3
                                articleId: articleId,
                                id: Ulid.NewUlid().ToString(),
                                rootId: commentId_1,
                                parentId: commentId_1,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-1-3"
                            ));
                            await commentRepository.Create(new Comment( // 1_1_1
                                articleId: articleId,
                                id: Ulid.NewUlid().ToString(),
                                rootId: commentId_1,
                                parentId: commentId_1_1,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-1-1-1"
                            ));
                            await commentRepository.Create(new Comment( // 1_1_2
                                articleId: articleId,
                                id: Ulid.NewUlid().ToString(),
                                rootId: commentId_1,
                                parentId: commentId_1_1,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-1-1-2"
                            ));
                            await commentRepository.Create(new Comment( // 1_1_3
                                articleId: articleId,
                                id: Ulid.NewUlid().ToString(),
                                rootId: commentId_1,
                                parentId: commentId_1_1,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-1-1-3"
                            ));
                            await commentRepository.Create(new Comment( // 1_1_4
                                articleId: articleId,
                                id: Ulid.NewUlid().ToString(),
                                rootId: commentId_1,
                                parentId: commentId_1_1,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-1-1-4"
                            ));

                            var commentId_2 = Ulid.NewUlid().ToString();
                            await commentRepository.Create(new Comment( // 2
                                articleId: articleId,
                                id: commentId_2,
                                rootId: commentId_2,
                                parentId: null,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-2"
                            ));

                            var commentId_3 = Ulid.NewUlid().ToString();
                            await commentRepository.Create(new Comment( // 3
                                articleId: articleId,
                                id: commentId_3,
                                rootId: commentId_3,
                                parentId: null,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-3"
                            ));
                            await commentRepository.Create(new Comment( // 3_1
                                articleId: articleId,
                                id: Ulid.NewUlid().ToString(),
                                rootId: commentId_3,
                                parentId: commentId_3,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-3-1"
                            ));
                            await commentRepository.Create(new Comment( // 3_2
                                articleId: articleId,
                                id: Ulid.NewUlid().ToString(),
                                rootId: commentId_3,
                                parentId: commentId_3,
                                authorId: userId,
                                authorUsername: username,
                                rating: 0,
                                body: "body-3-2"
                            ));
                        }
                    }

                    await unitOfWork.Commit();
                } catch {
                    await unitOfWork.Rollback();
                }
            }
        }
    }
}
