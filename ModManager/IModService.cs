using Modio;
using System;
using UnityEngine;

namespace Timberborn.ModsSystemUI {
  public interface IModService {

    ModsClient GetMods();

    GameTagsClient GetTags();

    Texture2D GetImage(Uri uri, int width, int height);

  }
}