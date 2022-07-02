﻿#nullable enable
using Microsoft.EntityFrameworkCore;
using PicturesAPI.Entities;
using PicturesAPI.Exceptions;
using PicturesAPI.Repos.Interfaces;

namespace PicturesAPI.Repos;

public class LikeRepo : ILikeRepo
{
    private readonly PictureDbContext _dbContext;

    public LikeRepo(PictureDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Like?> GetByIdAsync(int id)
    {
        return await _dbContext.Likes.SingleOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Like?> GetByLikerIdAndLikedIdAsync(int accountId, int pictureId)
    {
        return await _dbContext.Likes
            .Include(l => l.Account)
            .Include(l => l.Picture)
            .FirstOrDefaultAsync(l => l.AccountId == accountId && l.PictureId == pictureId);
    }

    public async Task<IEnumerable<Like>> GetByLikerIdAsync(int id)
    {
        return await _dbContext.Likes.Where(l => l.AccountId == id)
            .Include(a => a.Picture)
            .Include(a => a.Account)
            .ToArrayAsync();
    }

    public async Task<IEnumerable<Like>> GetByLikedIdAsync(int id)
    {
        return await _dbContext.Likes.Where(l => l.PictureId == id)
            .Include(a => a.Picture)
            .Include(a => a.Account)
            .ToArrayAsync();
    }

    public async Task<Like> InsertAsync(Like like)
    {
        await _dbContext.Likes.AddAsync(like);
        await _dbContext.SaveChangesAsync();
        return like;
    }

    public async Task<Like> DeleteByIdAsync(int id)
    {
        var like = await _dbContext.Likes.SingleOrDefaultAsync(l => l.Id == id) ?? throw new NotFoundException();
        _dbContext.Likes.Remove(like!);
        await _dbContext.SaveChangesAsync();
        return like;
    }

    public async Task<Like> UpdateAsync(Like like)
    {
        _dbContext.Likes.Update(like);
        await _dbContext.SaveChangesAsync();
        return like;
    }

}