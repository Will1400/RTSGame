using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface ISelectable
{
    bool IsSelected { get; set; }
    void Select();
    void Deselect();
}
