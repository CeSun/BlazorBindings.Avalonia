using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorBindingsAvalonia.Navigation;
public class BlazorPageTransition : IPageTransition
{
    public BlazorPageTransition()
    {
        Duration = TimeSpan.FromMilliseconds(300);
    }
    Easing Easing = new CubicEaseOut();

    /// <summary>
    /// 初始化 <see cref="CustomTransition"/> 类的新实例。
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    public BlazorPageTransition(TimeSpan duration)
    {
        Duration = duration;
    }

    /// <summary>
    /// 获取或设置动画的持续时间。
    /// </summary>
    public TimeSpan Duration { get; set; }

    public async Task Start(Visual from, Visual to, bool forward,
                                            CancellationToken cancellationToken)
    {
        if (to == null) return;

        // 计算原控件需要移动的距离（屏幕宽度的1/3）
        double moveDistance = from?.Bounds.Size.Width / 3 ?? 0;

        // 设置新控件的初始位置
        if (forward)
        {
            // 前进：目标页面从屏幕右侧进入
            to.RenderTransform = new TranslateTransform(from?.Bounds.Size.Width ?? 0, 0);
            from.ZIndex = 1;
            to.ZIndex = 2;
        }
        else
        {
            // 后退：目标页面从屏幕左侧进入
            to.RenderTransform = new TranslateTransform(-moveDistance, 0);
            from.ZIndex = 2;
            to.ZIndex = 1;
        }
        to.IsVisible = true;

        // 设置原控件的初始位置
        if (from != null)
        {
            from.RenderTransform = new TranslateTransform(0, 0);
        }

        // 创建新控件的动画
        var toAnimation = new Animation
        {
            Duration = Duration,
            Easing = Easing,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, forward ? (from?.Bounds.Size.Width ?? 0) : -moveDistance)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, 0d)
                    }
                }
            }
        };

        // 创建原控件的动画
        var fromAnimation = new Animation
        {
            Duration = Duration,
            Easing = Easing,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, 0d)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, forward ? -moveDistance : (from?.Bounds.Size.Width ?? 0))
                    }
                }
            }
        };

        // 并行执行两个动画
        var tasks = new List<Task>
        {
            toAnimation.RunAsync(to, cancellationToken)
        };
        if (from != null)
        {
            tasks.Add(fromAnimation.RunAsync(from, cancellationToken));
        }

        await Task.WhenAll(tasks);

        // 动画完成后，确保新控件完全可见，并隐藏原控件
        to.RenderTransform = null;
        if (from != null)
        {
            from.IsVisible = false;
            from.RenderTransform = null;
        }
    }
}

