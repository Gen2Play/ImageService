using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth;

public static class Permission
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
    public const string Upload = nameof(Upload);
}

public static class Resource
{
    public const string Image = nameof(Image);
    public const string Collection = nameof(Collection);
    public const string FavoriteImage = nameof(FavoriteImage);
    public const string Feedback = nameof(Feedback);
    public const string Tag = nameof(Tag);
    public const string Type = nameof(Type);
}
