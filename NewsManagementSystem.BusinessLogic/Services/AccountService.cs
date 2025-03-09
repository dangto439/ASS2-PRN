

using NewsManagementSystem.DataAccess.Models;
using NewsManagementSystem.DataAccess.Repositories;

namespace NewsManagementSystem.BusinessLogic.Services
{
    public class AccountService
    {
        private readonly IGenericRepository<SystemAccount> _userRepository;

        public AccountService(IGenericRepository<SystemAccount> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<short> Login(string email, string password)
        {
            var user = (await _userRepository.GetByDelegate(u => u.AccountEmail == email && u.AccountPassword == password)).FirstOrDefault();
            if (user == null) return -1;
            return user.AccountId;
        }
        public async Task<IEnumerable<SystemAccount>> GetAllAccont()
        {
            var users = await _userRepository.GetAll();
            return users;
        }
        public async Task<SystemAccount> GetAccount(short id)
        {
            var user = await _userRepository.GetById(id);
            return user;
        }
        public async Task AddAccount(SystemAccount user)
        {
            await _userRepository.Add(user);
        }
        public async Task UpdateAccount(SystemAccount user)
        {
            await _userRepository.Update(user);
        }
        public async Task<bool> DeleteAccount(short id)
        {
            return await _userRepository.Delete(id);
        }
        public async Task<short> GetNextId()
        {
            var users = await _userRepository.GetAll();
            return (short)(users.Max(u => u.AccountId) + 1);
        }

        /// <summary>
        /// Lấy danh sách AccountId
        /// </summary>
        public async Task<List<short>> GetAccountList()
        {
            var accounts = await _userRepository.GetAll();

            if (accounts == null || !accounts.Any())
            {
                return new List<short>(); 
            }

            return accounts.Select(a => a.AccountId).ToList();
        }

    }
}
