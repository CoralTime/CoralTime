using System;
using System.Collections.Generic;
using System.Text;

namespace CoralTime.ViewModels
{
    public class MemberAvatarPropertiesView
    {
        public int MemberId { get; set; }

        public byte[] AvatarFile { get; set; }

        public string AvatarFileName { get; set; }

        public byte[] ThumbnailFile { get; set; }
    }
}
