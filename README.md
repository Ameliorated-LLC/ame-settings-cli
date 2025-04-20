<h1 align="center">Privacy+ Settings</h1>
<h3 align="center">Privacy+ User & System Management Tool</h3>

<p align="center">
    <img src="screenshot.png?raw=true" width="450" alt="App Fetch Screenshot">
</p>
<h1></h1>

Tool for automating a large assortment actions on a Privacy+ system.

## Usage

You can download the tool by going to the [latest release](https://github.com/Ameliorated-LLC/ame-settings-legacy/releases/latest) and selecting `privacy+_settings.exe` from the **Downloads** section. Once downloaded, simply run `privacy+_settings.exe`.

## Summary

As part of the amelioration process, certain UI elements, such as the **Region and language** page in Windows Settings, have been removed, and require alternative ways to execute the tasks. This script fills in those gaps, and allows for automating a large variety of customization tasks.

If you prefer manually executing commands for some of these tasks without a script, you can go through the step-by-step guides on [AME Guides](https://t.me/AMEGuides).

## Primary Functions

There are many actions in AME that require commands or are otherwise unavailable. The following functions work as replacements for those actions.

#### Username/Password

This function allows for changing the user's username or password.

The following command can also be used for changing a username:

    wmic useraccount where "name='<User's Username>'" rename '<New Username>'

Similarly, the following command can be for changing the password:

    net user "<User's Username>" "<New Password>"

#### Lockscreen Image

This function allows for changing the lockscreen image.

It works by taking ownership of the existing profile image files, and replacing them with the new image supplied by the user.

#### Profile Image

This function allows for changing the user's profile image (PFP).

It does this by taking ownership of the existing profile image files, and replacing them with the new image supplied by the user. Several necessary registry changes are made as well.

#### User Elevation

This function allows for elevating or de-elevating the user to or from administrator. Elevating the user disables the password requirement when trying to run an executable as administrator. However, this has large security implications, thus why it is not the default setting.

The following command can also be used for this purpose:

    net localgroup administrators "<User's Username>" /add

Or the following for de-elevating the user:

    net localgroup administrators "<User's Username>" /delete

#### Keyboard Language

These functions allow for adding or removing a keyboard language.

At its core, this is done by using the following command:

    PowerShell -NoP -C "$NewLangs=Get-WinUserLanguageList; $NewLangs[0].InputMethodTips.Add('<Language/region ID>:<Keyboard Identifier>'); Set-WinUserLanguageList $NewLangs -Force"

If the user chose to make their selection the new default input method, the following command will also be run:

    PowerShell -NoP -C "Set-WinDefaultInputMethodOverride -InputTip "<Language/region ID>:<Keyboard Identifier>""

The Language/region ID and Keyboard identifier for a given language can be found [here](https://docs.microsoft.com/en-us/windows-hardware/manufacture/desktop/available-language-packs-for-windows?view=windows-11#language-packs) and [here](https://docs.microsoft.com/en-us/windows-hardware/manufacture/desktop/windows-language-pack-default-values?view=windows-11) respectively.

To remove a keyboard language, it fetches the existing language list, filters out the selected language, and sets the modified language list.

#### Username Login Requirement

This function allows for disabling or enabling the username login requirement.

The following command can also be used for this purpose:

    reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System" /v dontdisplaylastusername /f

Or the following for enabling the requirement:

    reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System" /v dontdisplaylastusername /t REG_DWORD /d 1 /f

#### AutoLogon

This function allows for enabling or disabling the automatic login of the current user.

It uses modified code from [rzander's AutoLogon program](https://github.com/rzander/AutoLogon), which can also be used for enabling AutoLogon.

#### .NET 3.5

This function allows for enabling or disabling .NET 3.5, requiring a Windows ISO or boot drive for installation.

## Extra Functions

This section contains beta, legacy, or less used functions. Legacy functions are only useful for versions of AME predating the [REDACTED].

#### Windows Subsystem for Linux (WSL)

These functions are currently unavailable.

#### Hibernation

This function allows for enabling or disabling the hibernation option in Windows.

At its core, the following commands are used:

    powercfg /HIBERNATE /TYPE FULL

Or the following for disabling hibernation:

    powercfg /HIBERNATE OFF

#### Notification Center

This function allows for enabling or disabling the Notification Center in the bottom right of the taskbar.

The following command can also be used for this purpose:

    reg add "HKU\<User's SID>\Software\Policies\Microsoft\Windows\Explorer" /v DisableNotificationCenter /t REG_DWORD /d 0 /f

Or the following for disabling the Notification Center:

    reg add "HKU\<User's SID>\Software\Policies\Microsoft\Windows\Explorer" /v DisableNotificationCenter /t REG_DWORD /d 1 /f

#### Desktop Notifications

This function allows for enabling or disabling desktop toast notifications.

The following command can also be used for this purpose:

    reg add "HKU\<User's SID>\SOFTWARE\Microsoft\Windows\CurrentVersion\PushNotifications" /v ToastEnabled /t REG_DWORD /d 1 /f

Or the following for disabling desktop notifications:

    reg add "HKU\<User's SID>\SOFTWARE\Microsoft\Windows\CurrentVersion\PushNotifications" /v ToastEnabled /t REG_DWORD /d 0 /f

## Known Issues

Some keyboard languages may not work, and a few may be improperly tagged.