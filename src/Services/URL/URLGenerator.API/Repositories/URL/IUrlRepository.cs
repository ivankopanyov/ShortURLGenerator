using ShortURLGenerator.URLGenerator.API.Models;
using URLGenerator.API.Repositories;

namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

public interface IUrlRepository : IRepository<Url, string> { }

