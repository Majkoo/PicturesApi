﻿#nullable enable
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PicturesAPI.Entities;
using PicturesAPI.Models.Dtos;
using PicturesAPI.Repos.Interfaces;
using PicturesAPI.Services.Helpers;

namespace PicturesAPI.Repos;

public class PictureRepo : IPictureRepo
{
    private readonly PictureDbContext _dbContext;

    public PictureRepo(
        PictureDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }

    public async Task<int> CountPicturesAsync(Expression<Func<Picture, bool>> predicate)
    {
        return await _dbContext.Pictures
            .Where(p => !p.IsDeleted)
            .Where(predicate)
            .CountAsync();
    }

    public async Task<Picture?> GetByIdAsync(int id)
    {
        return await _dbContext.Pictures
            .Where(p => !p.IsDeleted)
            .Include(p => p.Account)
            .ThenInclude(a => a.Role)
            .Include(p => p.PictureTagJoins)
            .ThenInclude(j => j.Tag)
            .Include(p => p.Likes)
            .ThenInclude(l => l.Liker)
            .Include(p => p.Comments
                .Where(c => c.IsDeleted == false))
            .ThenInclude(c => c.Account)
            .Include(p => p.Account)
            .AsSplitQuery()
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Picture>> GetFromAllAsync(int itemsToSkip, int itemsToTake)
    {
        return await _dbContext.Pictures
            .Where(p => !p.IsDeleted)
            .Include(p => p.Account)
            .ThenInclude(a => a.Role)
            .Include(p => p.PictureTagJoins)
            .ThenInclude(j => j.Tag)
            .Include(p => p.Likes)
            .ThenInclude(l => l.Liker)
            .Include(p => p.Comments
                .Where(c => c.IsDeleted == false))
            .ThenInclude(c => c.Account)
            .Include(p => p.Account)
            .OrderByDescending(p => p.PopularityScore)
            .Skip(itemsToSkip)
            .Take(itemsToTake)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Picture>> GetNotSeenByAccountIdAsync(int accountId, int itemsToTake)
    {
        return await _dbContext.Pictures
            .Where(p => !p.IsDeleted)
            .Where(p => !_dbContext.PictureSeenByAccountJoins
                .Where(j => j.AccountId == accountId)
                .AsSplitQuery()
                .Any(j => j.PictureId == p.Id && j.AccountId == accountId))
            .Include(p => p.Account)
            .ThenInclude(a => a.Role)
            .Include(p => p.PictureTagJoins)
            .ThenInclude(j => j.Tag)
            .Include(p => p.Likes)
            .ThenInclude(l => l.Liker)
            .Include(p => p.Comments
                .Where(c => c.IsDeleted == false))
            .ThenInclude(c => c.Account)
            .OrderByDescending(p => p.PopularityScore *
                                    (p.PictureTagJoins
                                         .Select(j =>
                                         j.Tag.AccountLikedTagJoins
                                             .OrderByDescending(t => t.Id)
                                             .Select(a => a.AccountId == accountId)
                                             .Take(15)
                                         )
                                         .Count() * 2.25)
            )
            .Take(itemsToTake)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Picture>> SearchAllAsync(int itemsToSkip, int itemsToTake, string searchPhrase)
    {
        return await _dbContext.Pictures
            .Where(p => !p.IsDeleted)
            .Where(p => searchPhrase == string.Empty || p.Name.ToLower().Contains(searchPhrase.ToLower()))
            .Include(p => p.Account)
            .ThenInclude(a => a.Role)
            .Include(p => p.PictureTagJoins)
            .ThenInclude(j => j.Tag)
            .Include(p => p.Likes)
            .ThenInclude(l => l.Liker)
            .Include(p => p.Comments
                .Where(c => c.IsDeleted == false))
            .ThenInclude(c => c.Account)
            .Include(p => p.Account)
            .OrderByDescending(p => p.PopularityScore)
            .Skip(itemsToSkip)
            .Take(itemsToTake)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Picture>> SearchNewestAsync(int itemsToSkip, int itemsToTake, string searchPhrase)
    {
        return await _dbContext.Pictures
            .Where(p => !p.IsDeleted)
            .Where(p => searchPhrase == string.Empty || p.Name.ToLower().Contains(searchPhrase.ToLower()))
            .Include(p => p.Account)
            .ThenInclude(a => a.Role)
            .Include(p => p.PictureTagJoins)
            .ThenInclude(j => j.Tag)
            .Include(p => p.Likes)
            .ThenInclude(l => l.Liker)
            .Include(p => p.Comments
                .Where(c => c.IsDeleted == false))
            .ThenInclude(c => c.Account)
            .Include(p => p.Account)
            .OrderByDescending(p => p.PictureAdded)
            .Skip(itemsToSkip)
            .Take(itemsToTake)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Picture>> SearchMostLikesAsync(int itemsToSkip, int itemsToTake, string searchPhrase)
    {
        return await _dbContext.Pictures
            .Where(p => !p.IsDeleted)
            .Where(p => searchPhrase == string.Empty || p.Name.ToLower().Contains(searchPhrase.ToLower()))
            .Include(p => p.Account)
            .ThenInclude(a => a.Role)
            .Include(p => p.PictureTagJoins)
            .ThenInclude(j => j.Tag)
            .Include(p => p.Likes)
            .ThenInclude(l => l.Liker)
            .Include(p => p.Comments
                .Where(c => c.IsDeleted == false))
            .ThenInclude(c => c.Account)
            .Include(p => p.Account)
            .OrderByDescending(p => p.Likes.Count(l => l.IsLike))
            .Skip(itemsToSkip)
            .Take(itemsToTake)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<Picture> InsertAsync(Picture picture)
    {
        await _dbContext.Pictures.AddAsync(picture);
        await _dbContext.SaveChangesAsync();
        return picture;
    }

    public async Task<Picture> UpdatePicScoreAsync (Picture picture)
    {
        _dbContext.Pictures.Update(picture);
        await _dbContext.SaveChangesAsync();
        return picture;
    }

    public async Task<Picture> UpdateAsync(Picture picture)
    {
        picture.PopularityScore = PictureScoreCalculator.CalcPoints(picture);
        _dbContext.Pictures.Update(picture);
        await _dbContext.SaveChangesAsync();
        return picture;
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var pic = _dbContext.Pictures.SingleOrDefault(p => p.Id == id);
        if (pic is null) return false;

        pic.IsDeleted = true;
        return await _dbContext.SaveChangesAsync() > 0;
    }
}