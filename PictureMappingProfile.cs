﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PicturesAPI.Entities;
using PicturesAPI.Models;

namespace PicturesAPI;

public class PictureMappingProfile : Profile
{
    public PictureMappingProfile()
    {

        CreateMap<Picture, PictureDto>()
            .ForMember(
                p => p.Tags,
                c => c
                    .MapFrom(s => s.Tags.Split()))
            .ForMember(
                p => p.Likes,
                m => m
                    .MapFrom(p => p.Likes.Count))
            .ForMember(
                p => p.Dislikes,
                m => m
                    .MapFrom(p => p.Dislikes.Count));

        CreateMap<Account, AccountDto>();

        CreateMap<CreateAccountDto, Account>()
            .ForMember(
                c => c.AccountCreated,
                a => a.MapFrom(
                    m => DateTime.Now));

        CreateMap<CreatePictureDto, Picture>()
            .ForMember(
                p => p.Tags,
                p => p
                    .MapFrom(c => string.Join(" ", c.Tags)));
    }
}