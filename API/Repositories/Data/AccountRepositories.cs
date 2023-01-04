using Microsoft.EntityFrameworkCore;
using API.Contexts;
using API.Entity;
using API.ViewModels;
using API.Repositories.Interface;
using API.Handlers;
using System.Collections;

namespace API.Repositories.Data;

public class AccountRepositories : GeneralRepository<Account, string>
{
    private MyContext _context;
    private DbSet<Account> _accounts;
    public AccountRepositories(MyContext context) : base(context)
    {
        _context = context;
        _accounts = context.Set<Account>();
    }

    /* private MyContext _context;
private DbSet<Account> _accounts;
public AccountRepositories(MyContext context)
{
   _context = context;
   _accounts = context.Set<Account>();
}

public int Delete(string id)
{
   var data = _accounts.Find(id);
   if (data == null)
   {
       return 0;
   }

   _accounts.Remove(data);
   var result = _context.SaveChanges();
   return result;
}


public IEnumerable<Account> Get()
{

   return _accounts.ToList();
}

public Account Get(string id)
{
   return _accounts.Find(id);
}

public int Insert(Account entity)
{
   _accounts.Add(entity);
   var result = _context.SaveChanges();
   return result;
}

public int Update(Account entity)
{
   _accounts.Entry(entity).State = EntityState.Modified;
   var result = _context.SaveChanges();
   return result;
} */

    public int Register(RegisterVM register)
    {
        register.NIK = GenerateNIK();

        if (!CheckEmailPhone(register.Email, register.Phone))
        {
            return 0; // Email atau Password sudah terdaftar
        }

        Employee employee = new Employee()
        {
            NIK = register.NIK,
            FirstName = register.FirstName,
            LastName = register.LastName,
            Phone = register.Phone,
            Gender = register.Gender,
            BirthDate = register.BirthDate,
            Salary = register.Salary,
            Email = register.Email,
        };
        _context.Employees.Add(employee);
        _context.SaveChanges();

        Account account = new Account()
        {
            NIK = register.NIK,
            Password = Hashing.HashPassword(register.Password),
        };
        _accounts.Add(account);
        _context.SaveChanges();

        University university = new University()
        {
            Name = register.UniversityName
        };
        _context.Universities.Add(university);
        _context.SaveChanges();

        Education education = new Education()
        {
            Degree = register.Degree,
            GPA = register.GPA,
            UniversityId = university.Id,
        };
        _context.Educations.Add(education);
        _context.SaveChanges();

        Profiling profiling = new Profiling()
        {
            NIK = register.NIK,
            EducationId = education.Id
        };
        _context.Profilings.Add(profiling);
        _context.SaveChanges();

        _context.AccountRoles.Add(new AccountRole()
        {
            AccountNIK = register.NIK,
            RoleId = 2
        });
        _context.SaveChanges();

        return 1;
    }

    public int Login(LoginVM login)
    {
        var result = _accounts.Join(_context.Employees, a => a.NIK, e => e.NIK, (a, e) =>
        new LoginVM
        {
            Email = e.Email,
            Password = a.Password
        }).SingleOrDefault(c => c.Email == login.Email);

        if (result == null)
        {
            return 0; // Email Tidak Terdaftar
        }
        if (!Hashing.ValidatePassword(login.Password, result.Password))
        {
            return 1; // Password Salah
        }
        return 2; // Email dan Password Benar
    }

    private bool CheckEmailPhone(string email, string phone)
    {
        var duplicate = _context.Employees.Where(s => s.Email == email || s.Phone == phone).ToList();

        if (duplicate.Any())
        {
            return false; // Email atau Password sudah ada
        }
        return true; // Email dan Password belum terdaftar
    }

    private string GenerateNIK()
    {
        var empCount = _context.Employees.OrderByDescending(e => e.NIK).FirstOrDefault();

        if (empCount == null)
        {
            return "x1111";
        }
        string NIK = empCount.NIK.Substring(1, 4);
        return Convert.ToString("x" + (Convert.ToInt32(NIK) + 1));
    }

    public List<string> UserRoles(string email)
    {
        /*        var result = _context.Employees
                .Join(_context.AccountRoles, e => e.NIK, ar => ar.AccountNIK, (e, ar) => new { e, ar })
                .Join(_context.Roles, ear => ear.ar.RoleId, ro => ro.Id, (ear, ro) => new { ear, ro })
                .Where(ear => ear.ear.e.Email == email)
                .Select(s => new AccountRoleVM
                {
                  RoleName = s.ro.Name
                }
                ).ToList();*/

        var result = (from e in _context.Employees
                      join a in _context.AccountRoles on e.NIK equals a.AccountNIK
                      join r in _context.Roles on a.RoleId equals r.Id
                      where e.Email == email
                      select r.Name);

        /*        var result = (from e in _context.Employees
                              select new
                              {
                                  name = (from userRole in _context.AccountRoles
                                          join role in _context.Roles on userRole.RoleId equals role.Id
                                          where e.Email == email
                                          select role.Name)
                              }).ToList();*/

        return new List<string>(result);
        
        // {"Employee", "Manager"}
        // {"Employee"}
    }
}