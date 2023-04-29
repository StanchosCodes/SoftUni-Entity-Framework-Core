namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            int producerId = 9;
            int duration = 4;

            string albumsInfo = ExportAlbumsInfo(context, producerId);
            string songsInfo = ExportSongsAboveDuration(context, duration);

            //Console.WriteLine(albumsInfo);
            Console.WriteLine(songsInfo);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder result = new StringBuilder();

            var albumsInfo = context.Albums
                 .Where(a => a.ProducerId == producerId)
                 .ToArray()
                 .OrderByDescending(a => a.Price)
                 .Select(a => new
                 {
                     AlbumName = a.Name,
                     ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                     ProducerName = a.Producer.Name,
                     Songs = a.Songs.Select(s => new
                     {
                         SongName = s.Name,
                         Price = s.Price.ToString("f2"),
                         WriterName = s.Writer.Name
                     })
                     .ToArray()
                     .OrderByDescending(s => s.SongName)
                     .ThenBy(s => s.WriterName),
                     TotalAlbumPrice = a.Price.ToString("f2")
                 })
                 .ToArray();

            foreach (var a in albumsInfo)
            {
                result.AppendLine($"-AlbumName: {a.AlbumName}")
                      .AppendLine($"-ReleaseDate: {a.ReleaseDate}")
                      .AppendLine($"-ProducerName: {a.ProducerName}")
                      .AppendLine("-Songs:");

                int songNumber = 1;

                foreach (var s in a.Songs)
                {
                    result.AppendLine($"---#{songNumber}")
                          .AppendLine($"---SongName: {s.SongName}")
                          .AppendLine($"---Price: {s.Price}")
                          .AppendLine($"---Writer: {s.WriterName}");

                    songNumber++;
                }

                result.AppendLine($"-AlbumPrice: {a.TotalAlbumPrice}");
            }

            return result.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder result = new StringBuilder();

            var songsInfo = context.Songs
                .ToArray()
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    SongName = s.Name,
                    Performers = s.SongPerformers.Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                                                 .OrderBy(p => p)
                                                 .ToArray(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ToArray();

            int songNumber = 1;

            foreach (var s in songsInfo)
            {
                result.AppendLine($"-Song #{songNumber}")
                      .AppendLine($"---SongName: {s.SongName}")
                      .AppendLine($"---Writer: {s.WriterName}");

                foreach (var performer in s.Performers)
                {
                    result.AppendLine($"---Performer: {performer}");
                }

                result.AppendLine($"---AlbumProducer: {s.AlbumProducer}")
                      .AppendLine($"---Duration: {s.Duration}");

                songNumber++;
            }

            return result.ToString().TrimEnd();
        }
    }
}
