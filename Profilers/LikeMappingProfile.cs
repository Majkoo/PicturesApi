﻿using AutoMapper;
using PicturesAPI.Entities;
using PicturesAPI.Models.Dtos;
using PicturesAPI.Services.Helpers;

namespace PicturesAPI.Profilers;

public class LikeMappingProfile : Profile
{
    public LikeMappingProfile()
    {
        CreateMap<Like, LikeDto>()
            .ForMember(dto => dto.AccountNickname,
                opt => opt.MapFrom(
                    l => l.Account.Nickname))
            .ForMember(dto => dto.AccountId,
                opt => opt.MapFrom(
                    l => IdHasher.EncodeAccountId(l.AccountId)))
            .ForMember(dto => dto.PictureId,
                opt => opt.MapFrom(
                    l => IdHasher.EncodePictureId(l.PictureId)));
    }
}