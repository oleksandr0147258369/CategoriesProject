using AutoMapper;
using WorkingMVC.Data.Entities;
using WorkingMVC.Interfaces;
using WorkingMVC.Models.Category;

namespace WorkingMVC.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository,
        IImageService imageService,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _imageService = imageService;
        _mapper = mapper;
    }

    // CREATE
    public async Task CreateAsync(CategoryCreateModel model)
    {
        var existing = await _categoryRepository.FindByNameAsync(model.Name);
        if (existing != null)
            throw new Exception($"Category with name '{model.Name}' already exists");

        var entity = new CategoryEntity
        {
            Name = model.Name
        };

        if (model.Image != null)
            entity.Image = await _imageService.UploadImageAsync(model.Image);

        await _categoryRepository.AddAsync(entity);
    }

    // READ ALL
    public async Task<List<CategoryItemModel>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllQueryableAsync();
        var model = _mapper.Map<List<CategoryItemModel>>(categories);
        return model;
    }

    // READ SINGLE
    public async Task<CategoryUpdateModel?> GetByIdAsync(int id)
    {
        var entity = await _categoryRepository.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            return null;

        return _mapper.Map<CategoryUpdateModel>(entity);
    }

    // UPDATE
    public async Task UpdateAsync(int id, CategoryUpdateModel model)
    {
        var entity = await _categoryRepository.GetByIdAsync(id);
        if (entity == null || entity.IsDeleted)
            throw new Exception("Category not found");

        entity.Name = model.Name;

        // If new image uploaded, replace
        if (model.Image != null)
        {
            // optional: remove old image file
            entity.Image = await _imageService.UploadImageAsync(model.Image);
        }

        await _categoryRepository.UpdateAsync(entity);
    }

    // DELETE (soft delete)
    public async Task DeleteAsync(int id)
    {
        var entity = await _categoryRepository.GetByIdAsync(id);
        if (entity == null)
            return;

        entity.IsDeleted = true;
        await _categoryRepository.UpdateAsync(entity);
    }
}