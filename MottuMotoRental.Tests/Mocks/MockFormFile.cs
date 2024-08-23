using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

public class MockFormFile : IFormFile
{
    private readonly Stream _stream;
    private readonly string _fileName;

    public MockFormFile(Stream stream, string fileName)
    {
        _stream = stream;
        _fileName = fileName;
    }

    public string ContentType => "image/png";
    public string ContentDisposition => $"inline; filename={_fileName}";
    
   
    public IHeaderDictionary Headers => new HeaderDictionaryStub();

    public long Length => _stream.Length;
    public string Name => "mockFile";
    public string FileName => _fileName;

    public void CopyTo(Stream target)
    {
        _stream.CopyTo(target);
    }

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        return _stream.CopyToAsync(target, cancellationToken);
    }

    public Stream OpenReadStream()
    {
        return _stream;
    }

    
    private class HeaderDictionaryStub : Dictionary<string, Microsoft.Extensions.Primitives.StringValues>, IHeaderDictionary
    {
        public long? ContentLength { get; set; }
    }
}