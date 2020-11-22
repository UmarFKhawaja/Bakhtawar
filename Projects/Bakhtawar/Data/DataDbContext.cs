using System;
using System.Linq;
using Bakhtawar.Models;
using Microsoft.EntityFrameworkCore;

namespace Bakhtawar.Data
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options)
            : base(options)
        {
        }

        public DbSet<Audio> Audios { get; set; }

        public DbSet<AudioPost> AudioPosts { get; set; }

        public DbSet<Gallery> Galleries { get; set; }

        public DbSet<GalleryPost> GalleryPosts { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<ImagePost> ImagePosts { get; set; }

        public DbSet<Keyword> Keywords { get; set; }

        public DbSet<Link> Links { get; set; }

        public DbSet<LinkPost> LinkPosts { get; set; }

        public DbSet<Option> Options { get; set; }

        public DbSet<Persona> Personas { get; set; }

        public DbSet<Poll> Polls { get; set; }

        public DbSet<PollPost> PollPosts { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostKeyword> PostKeywords { get; set; }

        public DbSet<PostTag> PostTags { get; set; }

        public DbSet<RoleClaim> RoleClaims { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<UserClaim> UserClaims { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserLogin> UserLogins { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Video> Videos { get; set; }

        public DbSet<VideoPost> VideoPosts { get; set; }

        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ConfigureAudioEntity()
                .ConfigureAudioPostEntity()
                .ConfigureGalleryEntity()
                .ConfigureGalleryPostEntity()
                .ConfigureImageEntity()
                .ConfigureImagePostEntity()
                .ConfigureKeywordEntity()
                .ConfigureLinkEntity()
                .ConfigureLinkPostEntity()
                .ConfigureOptionEntity()
                .ConfigurePersonaEntity()
                .ConfigurePollEntity()
                .ConfigurePollPostEntity()
                .ConfigurePostEntity()
                .ConfigurePostKeywordEntity()
                .ConfigurePostTagEntity()
                .ConfigureRoleEntity()
                .ConfigureRoleClaimEntity()
                .ConfigureTagEntity()
                .ConfigureUserEntity()
                .ConfigureUserClaimEntity()
                .ConfigureUserLoginEntity()
                .ConfigureUserRoleEntity()
                .ConfigureVideoEntity()
                .ConfigureVideoPostEntity()
                .ConfigureVoteEntity();
        }
    }

    public static class ContentDbContextExtensions
    {
        public static ModelBuilder ConfigureAudioEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Audio>()
                .ToTable("Audio");

            modelBuilder
                .Entity<Audio>()
                .Property((audio) => audio.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Audio>()
                .HasKey((audio) => audio.Id);

            modelBuilder
                .Entity<Audio>()
                .HasOne((audio) => audio.AudioPost)
                .WithOne((audioPost) => audioPost.Audio)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureAudioPostEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AudioPost>()
                .Property((audioPost) => audioPost.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<AudioPost>()
                .Property((audioPost) => audioPost.PersonaId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<AudioPost>()
                .Property((audioPost) => audioPost.AudioId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<AudioPost>()
                .HasMany((audioPost) => audioPost.PostTags)
                .WithOne((postTag) => postTag.Post as AudioPost)
                .IsRequired();

            modelBuilder
                .Entity<AudioPost>()
                .HasMany((audioPost) => audioPost.PostKeywords)
                .WithOne((postKeyword) => postKeyword.Post as AudioPost)
                .IsRequired();

            modelBuilder
                .Entity<AudioPost>()
                .HasOne((audioPost) => audioPost.Audio)
                .WithOne((audio) => audio.AudioPost)
                .HasForeignKey<AudioPost>((audioPost) => audioPost.AudioId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureGalleryEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Gallery>()
                .ToTable("Gallery");

            modelBuilder
                .Entity<Gallery>()
                .Property((gallery) => gallery.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Gallery>()
                .HasKey((gallery) => gallery.Id);

            modelBuilder
                .Entity<Gallery>()
                .HasOne((gallery) => gallery.GalleryPost)
                .WithOne((galleryPost) => galleryPost.Gallery)
                .IsRequired();

            modelBuilder
                .Entity<Gallery>()
                .HasMany((gallery) => gallery.Images)
                .WithOne((image) => image.Gallery)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureGalleryPostEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<GalleryPost>()
                .Property((galleryPost) => galleryPost.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<GalleryPost>()
                .Property((galleryPost) => galleryPost.PersonaId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<GalleryPost>()
                .Property((galleryPost) => galleryPost.GalleryId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<GalleryPost>()
                .HasMany((galleryPost) => galleryPost.PostTags)
                .WithOne((postTag) => postTag.Post as GalleryPost)
                .IsRequired();

            modelBuilder
                .Entity<GalleryPost>()
                .HasMany((galleryPost) => galleryPost.PostKeywords)
                .WithOne((postKeyword) => postKeyword.Post as GalleryPost)
                .IsRequired();

            modelBuilder
                .Entity<GalleryPost>()
                .HasOne((galleryPost) => galleryPost.Gallery)
                .WithOne((gallery) => gallery.GalleryPost)
                .HasForeignKey<GalleryPost>((galleryPost) => galleryPost.GalleryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureImageEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Image>()
                .ToTable("Image");

            modelBuilder
                .Entity<Image>()
                .Property((image) => image.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Image>()
                .Property((image) => image.GalleryId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Image>()
                .HasKey((image) => image.Id);

            modelBuilder
                .Entity<Image>()
                .HasOne((image) => image.ImagePost)
                .WithOne((imagePost) => imagePost.Image)
                .IsRequired();

            modelBuilder
                .Entity<Image>()
                .HasOne((image) => image.Gallery)
                .WithMany((gallery) => gallery.Images)
                .HasForeignKey((image) => image.GalleryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureImagePostEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ImagePost>()
                .Property((imagePost) => imagePost.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<ImagePost>()
                .Property((imagePost) => imagePost.PersonaId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<ImagePost>()
                .Property((imagePost) => imagePost.ImageId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<ImagePost>()
                .HasMany((imagePost) => imagePost.PostTags)
                .WithOne((postTag) => postTag.Post as ImagePost)
                .IsRequired();

            modelBuilder
                .Entity<ImagePost>()
                .HasMany((imagePost) => imagePost.PostKeywords)
                .WithOne((postKeyword) => postKeyword.Post as ImagePost)
                .IsRequired();

            modelBuilder
                .Entity<ImagePost>()
                .HasOne((imagePost) => imagePost.Image)
                .WithOne((image) => image.ImagePost)
                .HasForeignKey<ImagePost>((imagePost) => imagePost.ImageId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureKeywordEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Keyword>()
                .ToTable("Keyword");

            modelBuilder
                .Entity<Keyword>()
                .Property((keyword) => keyword.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Keyword>()
                .Property((keyword) => keyword.Name)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Keyword>()
                .HasKey((keyword) => keyword.Id);

            modelBuilder
                .Entity<Keyword>()
                .HasMany((keyword) => keyword.PostKeywords)
                .WithOne((postKeyword) => postKeyword.Keyword)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureLinkEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Link>()
                .ToTable("Link");

            modelBuilder
                .Entity<Link>()
                .Property((link) => link.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Link>()
                .HasKey((link) => link.Id);

            modelBuilder
                .Entity<Link>()
                .HasOne((link) => link.LinkPost)
                .WithOne((linkPost) => linkPost.Link)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureLinkPostEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<LinkPost>()
                .Property((linkPost) => linkPost.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<LinkPost>()
                .Property((linkPost) => linkPost.PersonaId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<LinkPost>()
                .Property((linkPost) => linkPost.LinkId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<LinkPost>()
                .HasMany((linkPost) => linkPost.PostTags)
                .WithOne((postTag) => postTag.Post as LinkPost)
                .IsRequired();

            modelBuilder
                .Entity<LinkPost>()
                .HasMany((linkPost) => linkPost.PostKeywords)
                .WithOne((postKeyword) => postKeyword.Post as LinkPost)
                .IsRequired();

            modelBuilder
                .Entity<LinkPost>()
                .HasOne((linkPost) => linkPost.Link)
                .WithOne((link) => link.LinkPost)
                .HasForeignKey<LinkPost>((linkPost) => linkPost.LinkId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureOptionEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Option>()
                .ToTable("Option");

            modelBuilder
                .Entity<Option>()
                .Property((option) => option.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Option>()
                .Property((option) => option.PollId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Option>()
                .HasKey((option) => option.Id);

            modelBuilder
                .Entity<Option>()
                .HasOne((option) => option.Poll)
                .WithMany((poll) => poll.Options)
                .HasForeignKey((option) => option.PollId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder
                .Entity<Option>()
                .HasMany((option) => option.Votes)
                .WithOne((vote) => vote.Option)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigurePersonaEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Persona>()
                .ToTable("Persona");

            modelBuilder
                .Entity<Persona>()
                .Property((persona) => persona.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Persona>()
                .Property((persona) => persona.UserId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Persona>()
                .HasKey((persona) => persona.Id);

            modelBuilder
                .Entity<Persona>()
                .HasOne((persona) => persona.User)
                .WithMany((user) => user.Personas)
                .HasForeignKey((persona) => persona.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder
                .Entity<Persona>()
                .HasMany((persona) => persona.Posts)
                .WithOne((post) => post.Persona)
                .IsRequired();

            modelBuilder
                .Entity<Persona>()
                .HasMany((persona) => persona.Votes)
                .WithOne((vote) => vote.Persona)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigurePollEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Poll>()
                .ToTable("Poll");

            modelBuilder
                .Entity<Poll>()
                .Property((poll) => poll.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Poll>()
                .HasKey((poll) => poll.Id);

            modelBuilder
                .Entity<Poll>()
                .HasOne((poll) => poll.PollPost)
                .WithOne((pollPost) => pollPost.Poll)
                .IsRequired();

            modelBuilder
                .Entity<Poll>()
                .HasMany((poll) => poll.Options)
                .WithOne((option) => option.Poll)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigurePollPostEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PollPost>()
                .Property((pollPost) => pollPost.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<PollPost>()
                .Property((pollPost) => pollPost.PersonaId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<PollPost>()
                .Property((pollPost) => pollPost.PollId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<PollPost>()
                .HasMany((pollPost) => pollPost.PostTags)
                .WithOne((postTag) => postTag.Post as PollPost)
                .IsRequired();

            modelBuilder
                .Entity<PollPost>()
                .HasMany((pollPost) => pollPost.PostKeywords)
                .WithOne((postKeyword) => postKeyword.Post as PollPost)
                .IsRequired();

            modelBuilder
                .Entity<PollPost>()
                .HasOne((pollPost) => pollPost.Poll)
                .WithOne((poll) => poll.PollPost)
                .HasForeignKey<PollPost>((pollPost) => pollPost.PollId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigurePostEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Post>()
                .ToTable("Post");

            modelBuilder
                .Entity<Post>()
                .Property((post) => post.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Post>()
                .Property((post) => post.PersonaId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Post>()
                .HasKey((post) => post.Id);

            modelBuilder
                .Entity<Post>()
                .HasOne((post) => post.Persona)
                .WithMany((persona) => persona.Posts)
                .HasForeignKey((post) => post.PersonaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder
                .Entity<Post>()
                .HasMany((post) => post.PostTags)
                .WithOne((postTag) => postTag.Post)
                .IsRequired();

            modelBuilder
                .Entity<Post>()
                .HasMany((post) => post.PostKeywords)
                .WithOne((postKeyword) => postKeyword.Post)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigurePostKeywordEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PostKeyword>()
                .ToTable("PostKeyword");

            modelBuilder
                .Entity<PostKeyword>()
                .Property((postKeyword) => postKeyword.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<PostKeyword>()
                .Property((postKeyword) => postKeyword.PostId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<PostKeyword>()
                .Property((postKeyword) => postKeyword.KeywordId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<PostKeyword>()
                .HasKey((postKeyword) => postKeyword.Id);

            modelBuilder
                .Entity<PostKeyword>()
                .HasOne((postKeyword) => postKeyword.Post)
                .WithMany((post) => post.PostKeywords)
                .HasForeignKey((postKeyword) => postKeyword.PostId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder
                .Entity<PostKeyword>()
                .HasOne((postKeyword) => postKeyword.Keyword)
                .WithMany((keyword) => keyword.PostKeywords)
                .HasForeignKey((postKeyword) => postKeyword.KeywordId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigurePostTagEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PostTag>()
                .ToTable("PostTag");

            modelBuilder
                .Entity<PostTag>()
                .Property((postTag) => postTag.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<PostTag>()
                .Property((postTag) => postTag.PostId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<PostTag>()
                .Property((postTag) => postTag.TagId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<PostTag>()
                .HasKey((postTag) => postTag.Id);

            modelBuilder
                .Entity<PostTag>()
                .HasOne((postTag) => postTag.Post)
                .WithMany((post) => post.PostTags)
                .HasForeignKey((postTag) => postTag.PostId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder
                .Entity<PostTag>()
                .HasOne((postTag) => postTag.Tag)
                .WithMany((tag) => tag.PostTags)
                .HasForeignKey((postTag) => postTag.TagId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureRoleEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Role>()
                .ToTable("Role");

            modelBuilder
                .Entity<Role>()
                .Property((role) => role.Id)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Role>()
                .Property((role) => role.Name)
                .IsRequired();

            modelBuilder
                .Entity<Role>()
                .Property((role) => role.NormalizedName)
                .IsRequired();

            modelBuilder
                .Entity<Role>()
                .Property((role) => role.ConcurrencyStamp)
                .IsConcurrencyToken()
                .IsRequired();

            modelBuilder
                .Entity<Role>()
                .HasKey((role) => role.Id);

            return modelBuilder;
        }

        public static ModelBuilder ConfigureRoleClaimEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<RoleClaim>()
                .ToTable("RoleClaim");

            modelBuilder
                .Entity<RoleClaim>()
                .Property((roleClaim) => roleClaim.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder
                .Entity<RoleClaim>()
                .Property((roleClaim) => roleClaim.RoleId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<RoleClaim>()
                .Property((roleClaim) => roleClaim.ClaimType)
                .IsRequired();

            modelBuilder
                .Entity<RoleClaim>()
                .Property((roleClaim) => roleClaim.ClaimValue);

            modelBuilder
                .Entity<RoleClaim>()
                .HasKey((roleClaim) => roleClaim.Id);

            modelBuilder
                .Entity<RoleClaim>()
                .HasOne((roleClaim) => roleClaim.Role)
                .WithMany((role) => role.RoleClaims)
                .HasForeignKey((roleClaim) => roleClaim.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureTagEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Tag>()
                .ToTable("Tag");

            modelBuilder
                .Entity<Tag>()
                .Property((tag) => tag.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Tag>()
                .Property((tag) => tag.Name)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Tag>()
                .Property((tag) => tag.Description)
                .HasMaxLength(2560)
                .IsRequired();

            modelBuilder
                .Entity<Tag>()
                .HasKey((tag) => tag.Id);

            modelBuilder
                .Entity<Tag>()
                .HasMany((tag) => tag.PostTags)
                .WithOne((postTag) => postTag.Tag)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureUserEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .ToTable("User");

            modelBuilder
                .Entity<User>()
                .Property((user) => user.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<User>()
                .Property((user) => user.UserName)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .Property((user) => user.NormalizedUserName)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .Property((user) => user.Email)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .Property((user) => user.NormalizedEmail)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .Property((user) => user.EmailConfirmed)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .Property((user) => user.PasswordHash);

            modelBuilder
                .Entity<User>()
                .Property((user) => user.PhoneNumber);

            modelBuilder
                .Entity<User>()
                .Property((user) => user.PhoneNumberConfirmed)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .Property((user) => user.TwoFactorEnabled)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .Property((user) => user.LockoutEnd);

            modelBuilder
                .Entity<User>()
                .Property((user) => user.LockoutEnabled)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .Property((user) => user.AccessFailedCount)
                .IsRequired();

            modelBuilder
                .Entity<User>()
                .HasKey((user) => user.Id);

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.UserName)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.NormalizedUserName)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.Email)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.NormalizedEmail)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.PhoneNumber)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasKey((user) => user.Id);

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.UserName)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.NormalizedUserName)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.Email)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.NormalizedEmail)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasIndex((user) => user.PhoneNumber)
                .IsUnique();

            modelBuilder
                .Entity<User>()
                .HasMany((user) => user.Personas)
                .WithOne((persona) => persona.User)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureUserClaimEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserClaim>()
                .ToTable("UserClaim");

            modelBuilder
                .Entity<UserClaim>()
                .Property((userClaim) => userClaim.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder
                .Entity<UserClaim>()
                .Property((userClaim) => userClaim.UserId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<UserClaim>()
                .Property((userClaim) => userClaim.ClaimType)
                .IsRequired();

            modelBuilder
                .Entity<UserClaim>()
                .Property((userClaim) => userClaim.ClaimValue);

            modelBuilder
                .Entity<UserClaim>()
                .HasKey((userClaim) => userClaim.Id);

            modelBuilder
                .Entity<UserClaim>()
                .HasOne((userClaim) => userClaim.User)
                .WithMany((user) => user.UserClaims)
                .HasForeignKey((userClaim) => userClaim.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureUserLoginEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserLogin>()
                .ToTable("UserLogin");

            modelBuilder
                .Entity<UserLogin>()
                .Property((userLogin) => userLogin.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder
                .Entity<UserLogin>()
                .Property((userLogin) => userLogin.LoginProvider)
                .IsRequired();

            modelBuilder
                .Entity<UserLogin>()
                .Property((userLogin) => userLogin.ProviderKey)
                .IsRequired();

            modelBuilder
                .Entity<UserLogin>()
                .Property((userLogin) => userLogin.ProviderDisplayName)
                .IsRequired();

            modelBuilder
                .Entity<UserLogin>()
                .Property((userLogin) => userLogin.UserId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<UserLogin>()
                .HasKey((userLogin) => userLogin.Id);

            modelBuilder
                .Entity<UserLogin>()
                .HasIndex((userLogin) => new { userLogin.LoginProvider, userLogin.ProviderKey })
                .IsUnique();

            modelBuilder
                .Entity<UserLogin>()
                .HasOne((userLogin) => userLogin.User)
                .WithMany((user) => user.UserLogins)
                .HasForeignKey((userLogin) => userLogin.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureUserRoleEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserRole>()
                .ToTable("UserRole");

            modelBuilder
                .Entity<UserRole>()
                .Property((userRole) => userRole.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            modelBuilder
                .Entity<UserRole>()
                .Property((userRole) => userRole.UserId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<UserRole>()
                .Property((userRole) => userRole.RoleId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<UserRole>()
                .HasKey((userRole) => userRole.Id);

            modelBuilder
                .Entity<UserRole>()
                .HasIndex((userRole) => new { userRole.UserId, userRole.RoleId })
                .IsUnique();

            modelBuilder
                .Entity<UserRole>()
                .HasOne((userRole) => userRole.User)
                .WithMany((user) => user.UserRoles)
                .HasForeignKey((userRole) => userRole.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder
                .Entity<UserRole>()
                .HasOne((userRole) => userRole.Role)
                .WithMany((role) => role.UserRoles)
                .HasForeignKey((userRole) => userRole.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureVideoEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Video>()
                .ToTable("Video");

            modelBuilder
                .Entity<Video>()
                .Property((video) => video.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Video>()
                .HasKey((video) => video.Id);

            modelBuilder
                .Entity<Video>()
                .HasOne((video) => video.VideoPost)
                .WithOne((videoPost) => videoPost.Video)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureVideoPostEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<VideoPost>()
                .Property((videoPost) => videoPost.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<VideoPost>()
                .Property((videoPost) => videoPost.PersonaId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<VideoPost>()
                .Property((videoPost) => videoPost.VideoId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<VideoPost>()
                .HasMany((videoPost) => videoPost.PostTags)
                .WithOne((postTag) => postTag.Post as VideoPost)
                .IsRequired();

            modelBuilder
                .Entity<VideoPost>()
                .HasMany((videoPost) => videoPost.PostKeywords)
                .WithOne((postKeyword) => postKeyword.Post as VideoPost)
                .IsRequired();

            modelBuilder
                .Entity<VideoPost>()
                .HasOne((videoPost) => videoPost.Video)
                .WithOne((video) => video.VideoPost)
                .HasForeignKey<VideoPost>((videoPost) => videoPost.VideoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }

        public static ModelBuilder ConfigureVoteEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Vote>()
                .ToTable("Vote");

            modelBuilder
                .Entity<Vote>()
                .Property((vote) => vote.Id)
                .HasMaxLength(256)
                .IsRequired();
            
            modelBuilder
                .Entity<Vote>()
                .Property((vote) => vote.PersonaId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Vote>()
                .Property((vote) => vote.OptionId)
                .HasMaxLength(256)
                .IsRequired();

            modelBuilder
                .Entity<Vote>()
                .HasKey((vote) => vote.Id);

            modelBuilder
                .Entity<Vote>()
                .HasOne((vote) => vote.Persona)
                .WithMany((persona) => persona.Votes)
                .HasForeignKey((vote) => vote.PersonaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder
                .Entity<Vote>()
                .HasOne((vote) => vote.Option)
                .WithMany((option) => option.Votes)
                .HasForeignKey((vote) => vote.OptionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            return modelBuilder;
        }
    }
}