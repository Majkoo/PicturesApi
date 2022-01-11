﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PicturesAPI.Models;

public class PutPictureDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Tags { get; set; }
        
    public string Url { get; set; }
}