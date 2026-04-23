using Microsoft.Maui.Controls.Shapes;

namespace BuhtaLink.Resources.Views;

public partial class NavigationMenu : ContentView
{

    public delegate void NavigationRequestHandler(string targetPage);
    public event NavigationRequestHandler NavigationRequested;

    public NavigationMenu()
	{
		InitializeComponent();
		
		//Создаем кнопки для нижнего меню
		var newsBtn = new ImageButton { Source = "news_icon.png", MaximumHeightRequest = 64, MaximumWidthRequest = 64, Aspect = Aspect.Fill, Padding = 12.5, };
		newsBtn.Clicked += (s, e) => NavigationRequested?.Invoke("news");
		var msgBtn = new ImageButton { Source = "msg_icon.png", MaximumHeightRequest = 64, MaximumWidthRequest = 64, Aspect = Aspect.Fill, Padding = 12.5 }; 
		msgBtn.Clicked += (s, e) => NavigationRequested?.Invoke("messages"); ;
        var profileBtn = new ImageButton { Source = "profile_icon.png", MaximumHeightRequest = 64, MaximumWidthRequest = 64, Aspect = Aspect.Fill, Padding = 12.5 };
		profileBtn.Clicked += (s, e) => NavigationRequested?.Invoke("profile"); ;
		//Создаем объект границы, внутри которого лежит навигационное меню
        this.Content = new Border
		{
			Stroke = Colors.Gray,
			Background = Color.FromArgb("#001e47"),
			StrokeShape = new RoundRectangle
			{
				CornerRadius = 25,
			},
			Content = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = { 
					newsBtn, 
					msgBtn, 
					profileBtn,
				}
			},
			MaximumWidthRequest = 252
		};
	}
}