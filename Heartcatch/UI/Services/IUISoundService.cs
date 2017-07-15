using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Heartcatch.UI.Services
{
    public interface IUISoundService
    {
        void OnClick();
        void OnDeny();
        void OnAccept();
    }
}
