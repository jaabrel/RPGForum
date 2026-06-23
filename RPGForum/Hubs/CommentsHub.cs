using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Crypto.Agreement;

namespace RPGForum.Hubs
{
    public class CommentsHub : Hub
    {

        public async Task JoinBuildGroup( int buildId)
        {
            await Groups.AddToGroutpAsync(ContextBoundObject.ConnectionId, $"build-{buildId}");
        }

        public async Task LeaveBuildGroup(int buildId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"build-{buildId}");
        }

        public async Task NotifyNewComment(int buildId, string userName, string commentText)
        {
            await Clients.Group($"build-{buildId}").SendAsync
                ("ReceiveComment", new
                {
                    userName = userName,
                    commentText = commentText,
                    timestamp = DateTime.UtcNow
                });
        }

        public async Task NotifyNewLike(int buildId, string userName)
        {
            await Clients.Group($"build-{buildId}").SendAsync("ReceiveLike", new
            {
                userName = userName,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task NotifyCommentDeleted(int buildId, int commentId)
        {
            await Clients.Group($"build-{buildId}").SendAsync("CommentDeleted", new
            {
                commentId = commentId,
                timestamp = DateTime.UtcNow
            });
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
