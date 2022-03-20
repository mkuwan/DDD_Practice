# Unit Testing Principles, Practices, and Patterns
`Vladimir Khorikov`

## �`���v�^�[�}�b�v
![Chapter Map](chapter-map.png)

### 4��
#### �P�̃e�X�g��4�̒�
- Protection against regressions (���O���b�V����[�o�O����]��h��)[^1]
- Resistance to refactoring (���t�@�N�^�����O�ɑς���)
- Fast feedback (�v���ȃt�B�[�h�o�b�N)
- Maintainability (�ێ琫)

[^1]:�\�t�g�E�F�A�J���̉ߒ��ŕs�����������v���O�������C������邱�Ƃ͂悭���邪�A���̏C���ɂ���Ă���܂Ő���ɓ��삵�Ă����������ُ���������悤�ɂȂ邱�Ƃ�����B���̂悤�Ȍ��ۂ��u�f�O���[�h�v�u���O���b�V�����v�ȂǂƂ���

### �e�X�g�_�u���ɂ�5�̃o���G�[�V����������܂�
![File](file.png)
- ���b�N�́A����̃C���^���N�V�������G�~�����[�g���Ē��ׂ�̂ɖ𗧂��܂��B�����̑��ݍ�p�́ASUT���ˑ��֌W�ɑ΂��čs���ď�Ԃ�ύX���邽�߂̌Ăяo���ł��B
- �X�^�u�́A���M�C���^���N�V�������G�~�����[�g����̂ɖ𗧂��܂��B�����̑��ݍ�p�́A���̓f�[�^���擾���邽�߂�SUT���ˑ��֌W�ɑ΂��čs���Ăяo���ł�


### DDD
- ���܂��܂ȃ��C���[�œ��삷��e�X�g�ɂ̓t���N�^����������܂��B���܂��܂ȃ��x���œ�����������؂��܂��B�A�v���P�[�V�����T�[�r�X�̃e�X�g�ł́A�r�W�l�X���[�X�P�[�X�S�̂��ǂ̂悤�Ɏ��s����邩���m�F���܂��B�h���C���N���X���g�p����e�X�g�ł́A���[�X�P�[�X�̊����Ɍ����Ē��Ԃ̃T�u�S�[�������؂��܂��B

![Ddd](ddd.png)


- �K�؂ɐ݌v���ꂽAPI���g�p���ăR�[�h�x�[�X�����؂���e�X�g�́A�ώ@�\�ȓ���ɂ̂݊֘A���邽�߁A�r�W�l�X�v���ɂ��֘A���Ă��܂�
```C#
public class User
{
    private string _name;
    public string Name
    {
        get => _name;
        set => _name = NormalizeName(value);
    }

    private string NormalizeName(string name)
    {
        /* Trim name down to 50 characters */
    }
}

public class UserController
{
    public void RenameUser(int userId, string newName)
    {
        User user = GetUserFromDatabase(userId);
        user.Name = newName;
        SaveUserToDatabase(user);
    }
}
```

### 5.3.2 �V�X�e�����ʐM�ƃV�X�e���ԒʐM

��ʓI�ȃA�v���P�[�V�����ɂ́A�V�X�e�����ƃV�X�e���ԂƂ���2��ނ̒ʐM������܂��B�V�X�e�����ʐM�́A�A�v���P�[�V�������̃N���X�Ԃ̒ʐM�ł��B�V�X�e���ԒʐM�́A�A�v���P�[�V���������̃A�v���P�[�V�����ƒʐM����Ƃ��ł�

![Inter Intra System](inter-intra-system.png)

�V�X�e���ԒʐM�́A�A�v���P�[�V�����S�̂̊ώ@�\�ȓ�����`�����܂��B�V�X�e�����ʐM�͎����̏ڍׂł�
![Inter Intra System2](inter-intra-system2.png)

### 6��

�P�̃e�X�g�̎��
- �o�̓x�[�X
- ��ԃx�[�X
- �R���{���[�V�����x�[�X

#### �o�̓x�[�X
![Output Verification](output-verification.png)
```C#
public class PriceEngine
{
    public decimal CalculateDiscount(params Product[] products)
    {
        decimal discount = products.Length * 0.01m;
        return Math.Min(discount, 0.2m);
    }
}

[Fact]
public void Discount_of_two_products()
{
    var product1 = new Product("Hand wash");
    var product2 = new Product("Shampoo");
    var sut = new PriceEngine();

    decimal discount = sut.CalculateDiscount(product1, product2);

    Assert.Equal(0.02m, discount);
}
```
#### ��ԃx�[�X
![State Verification](state-verification.png)
```C#
public class Order
{
    private readonly List<Product> _products = new List<Product>();
    public IReadOnlyList<Product> Products => _products.ToList();

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }
}

[Fact]
public void Adding_a_product_to_an_order()
{
    var product = new Product("Hand wash");
    var sut = new Order();

    sut.AddProduct(product);

    Assert.Equal(1, sut.Products.Count);
    Assert.Equal(product, sut.Products[0]);
}
```
#### �R���{���[�V�����x�[�X
![Collaboration Verification](collaboration-verification.png)
```C#
[Fact]
public void Sending_a_greetings_email()
{
    var emailGatewayMock = new Mock<IEmailGateway>();
    var sut = new Controller(emailGatewayMock.Object);

    sut.GreetUser("user@email.com");

    emailGatewayMock.Verify(
        x => x.SendGreetingsEmail("user@email.com"),
        Times.Once);
}
```

### 8��
#### �����e�X�g�ƒP�̃e�X�g�͈̔�
�P�̃e�X�g�̓h���C�����f����ΏۂƂ��A�����e�X�g�͂��̃h���C�����f�����A�E�g�v���Z�X�̈ˑ��֌W�ɐڒ�����R�[�h���`�F�b�N���܂��B
![Unit Integration](unit-integration.png)

### 10��
#### �f�[�^�x�[�X���g�p�����e�X�g
##### 10.3.2�B�e�X�g���s�Ԃ̃f�[�^�̃N���A
�e�X�g�̊J�n���Ƀf�[�^���N���[���A�b�v����

���ꂪ�ŗǂ̃I�v�V�����ł��B�����ɓ��삵�A����Ɉ�ѐ����Ȃ��A�N���[���A�b�v�t�F�[�Y������ăX�L�b�v����X��������܂���

##### 10.3.3�B�C���������f�[�^�x�[�X�������
�C���������f�[�^�x�[�X�͋��L�̈ˑ��֌W�ł͂Ȃ����߁A�Z�N�V����10.3.1�Ő��������R���e�i���g�p�����A�v���[�`�Ɠ��l�ɁA�����e�X�g�͎�����P�̃e�X�g�ɂȂ�܂�

### 11��
####
##### 11.3 �h���C���m�����e�X�g�ɘR�炷
�A���`�p�^�[��
- �e�X�g���{�ԃR�[�h����A���S���Y���̎����𕡐����Ă��܂��Ă���

