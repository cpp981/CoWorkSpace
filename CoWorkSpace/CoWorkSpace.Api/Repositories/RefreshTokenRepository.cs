using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.Models;

namespace CoWorkSpace.Api.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly CoWorkSpaceContext _context;

        public RefreshTokenRepository(CoWorkSpaceContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByHashAsync(string hash)
        {
            return await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(rt => rt.TokenHash == hash);
        }

        public async Task RevokeAsync(RefreshToken token, string? replacedByHash = null)
        {
            // Cargar la entidad en el contexto si viene desconectada
            var existing = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Id == token.Id);
            if (existing == null) return;

            existing.Revoked = true;
            existing.RevokedAt = DateTime.UtcNow;
            existing.ReplacedByTokenHash = replacedByHash;

            _context.RefreshTokens.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveByUserAsync(int userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.Revoked && rt.Expires > DateTime.UtcNow)
                .ToListAsync();
        }

        // Método útil opcional: revocar todos los tokens de un usuario (por ejemplo en detectReuse)
        public async Task RevokeAllForUserAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.Revoked)
                .ToListAsync();

            if (!tokens.Any()) return;

            foreach (var t in tokens)
            {
                t.Revoked = true;
                t.RevokedAt = DateTime.UtcNow;
            }

            _context.RefreshTokens.UpdateRange(tokens);
            await _context.SaveChangesAsync();
        }
    }
}

