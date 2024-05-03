using EM.DOMAIN;
using EM.DOMAIN.Servicos;
using EM.REPOSITORY;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IRepositorioAluno<Aluno>, RepositorioAluno>();
builder.Services.AddTransient<IRepositorioCidade<CidadeModel>, RepositorioCidade>();
builder.Services.AddTransient<GeradorRelatorioAluno>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
