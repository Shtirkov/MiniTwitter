using Microsoft.EntityFrameworkCore;
using MiniTwitter;
using MiniTwitter.Interfaces;
using MiniTwitter.Models;
using MiniTwitter.RequestModels;

public class CommentsService : ICommentsService
{
    private readonly TwitterContext _context;

    public CommentsService(TwitterContext context)
    {
        _context = context;
    }
    private IQueryable<Comment> CommentsQuery()
    {
        return _context.Comments.AsNoTracking().Include(c => c.Author);
    }

    private Task<Comment?> GetCommentByIdAsync(int id)
    {
       return CommentsQuery().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
    }

    public Comment Edit(Comment comment, EditCommentRequestDto commentDto)
    {
        comment.Content = commentDto.Content;
        return comment;
    }

    public async Task<Comment?> GetAsync(int id)
    {
        return await GetCommentByIdAsync(id);
    }

    public void Remove(Comment comment)
    {
        _context.Remove(comment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
