// ini buat kode redeem randomizer nanti yaa, 
// private static string GenerateRedeemCode()
// {
//     const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
//     var random = new Random();
//     string randomPart = new string(Enumerable.Repeat(chars, 6)
//         .Select(s => s[random.Next(s.Length)]).ToArray());
//     return $"RDM-{randomPart}";
// }

// Kode digenerate ulang setiap tiket di dalam loop
// foreach (var ticketId in ticketIds)
// {
//     string uniqueCode = GenerateRedeemCode(); // Tiket 1: "RDM-ABC123", Tiket 2: "RDM-XYZ789"
//     UpdateTicketToPaid(ticketId, uniqueCode);
// }