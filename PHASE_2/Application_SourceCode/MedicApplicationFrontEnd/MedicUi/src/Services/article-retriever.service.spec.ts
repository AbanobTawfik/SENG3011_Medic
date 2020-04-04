import { TestBed } from '@angular/core/testing';

import { ArticleRetrieverService } from './article-retriever.service';

describe('ArticleRetrieverService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ArticleRetrieverService = TestBed.get(ArticleRetrieverService);
    expect(service).toBeTruthy();
  });
});
