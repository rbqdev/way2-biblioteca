﻿namespace Domain.Services.Settings
{
    public interface IAppSettingsServices
    {
        string Secret { get; set; }
        string JsonBookFilePath { get; }
    }
}