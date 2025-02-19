﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Image;

public class CollectionByIDResponse
{
    public Guid CollectionID { get; set; }
    public Guid UserID { get; set; }
    public string Name { get; set; }
    public bool IsPublic { get; set; }
    public List<ImageListResponse>? Images { get; set; }
}
