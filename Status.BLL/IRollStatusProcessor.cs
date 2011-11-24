using System;
using Status.Model;

namespace Status.BLL
{
    public interface IRollStatusProcessor
    {
        StatusItem MapStatusItem(StatusItem sourceStatusItem, DateTime statusReportDate);
    }
}