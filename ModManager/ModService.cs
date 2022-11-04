using Modio;
using System;
using System.Net.Http;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.ModsSystemUI {
  public class ModService : IModService,
                            ILoadableSingleton {

    private GameClient _client;

    public void Load() {
      var client = new Client(new("7f52d134de5cde63fdcf163478e688e3"));
      _client = client.Games[3659];
    }

    public ModsClient GetMods() {
      return _client.Mods;
    }

    public GameTagsClient GetTags() {
      return _client.Tags;
    }

    public Texture2D GetImage(Uri uri, int width, int height) {
      using var client = new HttpClient();
      using var byteArray = client.GetByteArrayAsync(uri);
      var texture = new Texture2D(width, height);
      texture.LoadImage(byteArray.Result);
      return texture;
    }

  }
}