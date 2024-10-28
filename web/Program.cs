var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// �V�A�Ȯe�����K�[ MVC controller �M view �䴩�C
// �o�˴N�������ε{���ϥ� MVC �Ҧ��A�ӳB�z request �M�e�{ view �C
// �p�G���ε{���O MVC �� Razor Pages ���ءA�o�@��O�������C
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.

// �W�j���ε{�������~�B�z�M�w���ʡA����}�o���Ҩӻ����O�������C
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // �i�D�s�����u��H HTTPS �q�H�A�O�@�ئw���j����C
    app.UseHsts();
}

//app.UseDeveloperExceptionPage();

// �@�ΡG�j��ϥ� HTTPS ��ĳ�A�N�Ҧ� HTTP �ШD�۰ʭ��w�V�� HTTPS�C
// ���n�ʡG�b�Ͳ����Ҥ��ϥ� HTTPS �O���n���A�o��i�H�j�� HTTPS �s���C
app.UseHttpsRedirection();

// �@�ΡG�ҥ��R�A�ɮפ����n��A�H���� wwwroot �ؿ��U���R�A�귽�]�p�Ϥ��BCSS�BJavaScript�^�C
// ���n�ʡG�p�G���ε{�����R�A�귽�A�o��O���n���C
// ���N�G�Y���ε{�����ϥ��R�A�귽�]�p API �M�ס^�A�o��i�H�ٲ��C
app.UseStaticFiles();

// �@�ΡG�ҥθ��Ѥ����n��A�����ε{���̾کw�q�����ѨӳB�z�ШD�C�������U ASP.NET Core �ھڱ���B�ʧ@�M�����B�z�ШD�C
// ���n�ʡG�Y�n�b���ε{�����ϥθ��ѡ]�p MVC ���ѩ� API ���ѡ^�A�o��O�������C
// ���N�GAPI �� MVC ���صL�k�ٲ��o��A���i�H�b�򥻭������Τ��ٲ��C
app.UseRouting();

// �@�ΡG�ҥα��v�����n��A�T�O�u���g���v���Τ�i�H�s���S�w�귽�C
// ���n�ʡG�p�G���ε{���]�t����ݭn�������Ҫ������� API�A����O�������C
// ���N�G�p�G���ݭn�������ҩα��v�]�Ҧp�}�� API �Τ��������^�A�i�H�ٲ��o��C
//app.UseAuthorization();

// �@�ΡG�]�m�w�]�����ѽd���A���w�F URL ���|���c�A�䤤 {controller}�B{action} �M�i�諸 {id} ��ܱ���B�ʧ@�M ID �ѼơC
// ���n�ʡG�p�G�ϥ� MVC ����A�o��O�������A�Ω�M�g���ѡC
// ���N�G�i�H������ app.MapDefaultControllerRoute(); �ӨϥάۦP���w�]���ѳW�h�A²�Ƶ{���X�C
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();