﻿using System;
using System.Collections.Generic;

namespace PicturesAPI.Models;

public class PagedResult<T>
{
    public PagedResult(List<T> items, int totalItemsCount, int pageSize, int pageNumber)
    {
        Items = items;
        TotalItemsCount = totalItemsCount;
        ItemsFrom = pageSize * (pageNumber - 1) + 1;
        ItemsTo = (ItemsFrom + pageSize - 1 > totalItemsCount) 
            ? totalItemsCount 
            : ItemsFrom + pageSize - 1;
        TotalPages = (int)Math.Ceiling(totalItemsCount /(double) pageSize);
    }
    
    public List<T> Items { get; set; }
    public int TotalPages { get; set; }
    public int ItemsFrom { get; set; }
    public int ItemsTo { get; set; }
    public int TotalItemsCount { get; set; }
    
}