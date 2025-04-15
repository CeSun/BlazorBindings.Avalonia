# 🪢 Blazonia


## 🤔 这是啥子？

Blazonia可以让开发者使用 **<a href="https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor">Blazor</a>的语法**来开发 **<a href="https://avaloniaui.net/">Avalonia</a>程序**。相比原版Avalonia的axmal语法， Blazonia使用的Blazor语法更加的简洁和简单，只需要**单个文件**即可完成页面的开发。适用于**中小型规模**的客户端程序。

✨ **请注意**
- 🚫 使用Blazonia后**不会有**任何的HTML代码以及Webview组件
- 🤩 **完全使用**原生Avalonia控件

得益于Avalonia跨平台的特性，Blazonia可以让开发者快速的编写出**漂亮**的**像素完美级**的 💻 **电脑, 📱 手机 和 🌐 网页 应用程序**


## 🌰 这是一个小例子


```razor
@page "/"

<Window Title="Counter" Width="600" Height="400">
    <StackPanel>
        <Label FontSize="30">You pressed @_count times </Label>
        <CheckBox @bind-IsChecked="_showButton">Button visible</CheckBox>
        @if (_showButton! == true)
        {
            <Button OnClick="OnButtonClick">+1</Button>
        }
    </StackPanel>
</Window>

@code {

    int _count;
    
    bool? _showButton = true;
    
    void OnButtonClick() => _count++;
}
```

![Counter](/images/Blazonia.png "Counter")


## 📄 说明

Blazonia复刻自[https://github.com/Epictek/Avalonia-Blazor-Bindings](https://github.com/Epictek/Avalonia-Blazor-Bindings)分支，经过Micosoft、Dreamescaper、warappa和Epictek等开发者维护。

Blazor的语法入门门槛极低，对新手十分友好，但是原仓库看起来并不是活跃，所以我将继续维护该仓库，由于原名称在nuget上已经占用，我将其更名Blazonia(Blazor + Avalonia)，以便于后续上传nuget库和项目模板🙂。